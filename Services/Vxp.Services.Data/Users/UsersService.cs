using System;
using System.Linq.Expressions;

namespace Vxp.Services.Data.Users
{
    using Mapping;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using Vxp.Services.Models.Administration.Users;

    public class UsersService : IUsersService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDeletableEntityRepository<ApplicationUser> _usersRepository;
        private readonly IDeletableEntityRepository<Country> _countriesRepository;
        private readonly IDistributorsService _distributorsService;

        public UsersService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IDeletableEntityRepository<Country> countriesRepository,
            IDistributorsService distributorsService)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._usersRepository = usersRepository;
            this._countriesRepository = countriesRepository;
            this._distributorsService = distributorsService;
        }

        public IQueryable<TViewModel> GetAll<TViewModel>(Expression<Func<ApplicationUser, bool>> exp)
        {
            var query = this._usersRepository.AllAsNoTracking()
                .Include(user => user.Roles)
                .Include(user => user.BankAccounts)
                .Include(user => user.Distributors)
                .ThenInclude(distributorUser => distributorUser.DistributorKey)
                .ThenInclude(distributorKey => distributorKey.BankAccount)
                .ThenInclude(bankAccount => bankAccount.Owner)
                .ThenInclude(applicationUser => applicationUser.Company).AsQueryable();

            if (exp != null)
            {
                query = query.Where(exp);
            }

            return query.To<TViewModel>();
        }

        public async Task<IEnumerable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName)
        {
            var role = await this._roleManager.FindByNameAsync(roleName);

            return await this._usersRepository.AllAsNoTracking().Where(u => u.Roles.Any(r => r.RoleId == role.Id))
                .To<TViewModel>()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllCountriesAsync()
        {
            return await this._countriesRepository
                .AllAsNoTrackingWithDeleted()
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToListAsync();
        }

        public async Task<string> CreateUser<TViewModel>(TViewModel userModel)
        {

            var inputUsers = new HashSet<TViewModel> { userModel };
            var serviceUsers = inputUsers.AsQueryable().To<CreateUserServiceModel>();
            var applicationUsers = serviceUsers.To<ApplicationUser>();

            var serviceUser = serviceUsers.First();
            var applicationUser = applicationUsers.First();

            if (applicationUser.ContactAddress != null)
            {
                var countryFromDb = this._countriesRepository
                    .AllAsNoTrackingWithDeleted()
                    .FirstOrDefault(c => c.Name == serviceUser.Country);

                if (countryFromDb == null) // seed
                {
                    countryFromDb = new Country
                    {
                        Name = serviceUser.Country,
                        Language = serviceUser.Country
                    };

                    await this._countriesRepository.AddAsync(countryFromDb);
                }

                applicationUser.ContactAddress.CountryId = countryFromDb.Id;
            }

            string newDistributorKey = null;

            if (!string.IsNullOrWhiteSpace(serviceUser.DistributorId))
            {
                var distributor = await this._userManager.FindByIdAsync(serviceUser.DistributorId);
                if (distributor == null)
                {
                    return null;
                }

                newDistributorKey = await this._distributorsService.GenerateNewDistributorKeyAsync(distributor.UserName);
            }

            var user = await this._userManager.CreateAsync(applicationUser, serviceUser.Password);

            if (user.Succeeded)
            {
                await this._userManager.AddToRoleAsync(applicationUser, serviceUser.Role);

                if (!string.IsNullOrWhiteSpace(newDistributorKey))
                {
                    if (!await this._distributorsService.AddCustomerToDistributor(applicationUser.UserName, newDistributorKey))
                    {
                        await this._userManager.DeleteAsync(applicationUser);
                        return null;
                    }
                }

                return applicationUser.Id;
            }

            return null;
        }
    }
}
