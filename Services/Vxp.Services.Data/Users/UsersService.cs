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
    using System;
    using System.Linq.Expressions;

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

        public Task<IQueryable<TViewModel>> GetAll<TViewModel>(Expression<Func<ApplicationUser, bool>> exp)
        {
            var query = this._usersRepository.AllAsNoTracking()
                .Include(user => user.Company)
                .Include(user => user.Roles)
                .ThenInclude(role => role.Role)
                .Include(user => user.BankAccounts)
                .Include(user => user.Distributors)
                .ThenInclude(distributorUser => distributorUser.DistributorKey)
                .ThenInclude(distributorKey => distributorKey.BankAccount)
                .ThenInclude(bankAccount => bankAccount.Owner)
                .ThenInclude(bankAccountOwner => bankAccountOwner.Company).AsQueryable();

            if (exp != null)
            {
                query = query.Where(exp);
            }

            return Task.Run(() => query.To<TViewModel>());
        }

        public Task<IQueryable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName)
        {
            var applicationUsersInRole = this._usersRepository.AllAsNoTracking()
                .Where(u => u.Roles.Any(r => r.Role.Name == roleName));

            return Task.Run(() => applicationUsersInRole.To<TViewModel>());
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

        public bool UpdateUser<TViewModel>(TViewModel userModel)
        {
            var applicationUser = AutoMapper.Mapper.Map<ApplicationUser>(userModel);

            var userFromDb = this._usersRepository.AllWithDeleted().FirstOrDefault(u => u.Id == applicationUser.Id);

            if (userFromDb == null)
            {
                return false;
            }

            userFromDb.FirstName = applicationUser.FirstName;
            userFromDb.LastName = applicationUser.LastName;

            this._usersRepository.Update(userFromDb);
            this._usersRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return true;
        }
    }
}
