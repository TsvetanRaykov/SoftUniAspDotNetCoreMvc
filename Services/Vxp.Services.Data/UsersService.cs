using Vxp.Services.Models.Administration.Users;

namespace Vxp.Services.Data
{
    using Microsoft.AspNetCore.Identity;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using Mapping;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> _usersRepository;
        private readonly IDeletableEntityRepository<Country> _countriesRepository;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IDeletableEntityRepository<Country> countriesRepository,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this._usersRepository = usersRepository;
            this._countriesRepository = countriesRepository;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        public async Task<IEnumerable<TViewModel>> GetAllAsync<TViewModel>()
        {
            return await this._usersRepository.AllAsNoTracking()
                .To<TViewModel>()
                .ToListAsync();
        }

        public async Task<IEnumerable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName)
        {
            var role = await this._roleManager.FindByNameAsync(roleName);

            return await this._usersRepository.AllAsNoTracking().Where(u => u.Roles.Any(r => r.RoleId == role.Id))
                .To<TViewModel>()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllCountries()
        {
            return await this._countriesRepository
                .AllAsNoTrackingWithDeleted()
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToListAsync();
        }

        public Task<string> CreateUser(CreateUserServiceModel userModel)
        {

            throw new System.NotImplementedException();
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
                var countryCandidate = this._countriesRepository
                    .AllAsNoTrackingWithDeleted()
                    .FirstOrDefault(c => c.Name == serviceUser.Country);

                if (countryCandidate == null) // seed
                {
                    countryCandidate = new Country
                    {
                        Name = serviceUser.Country,
                        Language = serviceUser.Country
                    };

                    await this._countriesRepository.AddAsync(countryCandidate);
                }

                applicationUser.ContactAddress.CountryId = countryCandidate.Id;
            }


            var user = await this._userManager.CreateAsync(applicationUser, serviceUser.Password);

            if (user.Succeeded)
            {
                await this._userManager.AddToRoleAsync(applicationUser, serviceUser.Role);

                return applicationUser.Id;
            }

            return null;
        }
    }
}
