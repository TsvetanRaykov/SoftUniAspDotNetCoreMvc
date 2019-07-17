namespace Vxp.Services.Data
{
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
        private readonly IDeletableEntityRepository<ApplicationRole> _rolesRepository;
        private readonly IDeletableEntityRepository<Country> _countriesRepository;

        private Dictionary<string, string> _roles;

        private Dictionary<string, string> Roles =>
            this._roles ?? (this._roles = this._rolesRepository.AllAsNoTrackingWithDeleted()
                                                        .ToDictionaryAsync(key => key.Name, v => v.Id)
                                                        .GetAwaiter().GetResult());

        public UsersService(IDeletableEntityRepository<ApplicationUser> usersRepository, IDeletableEntityRepository<ApplicationRole> rolesRepository, IDeletableEntityRepository<Country> countriesRepository)
        {
            this._usersRepository = usersRepository;
            this._rolesRepository = rolesRepository;
            this._countriesRepository = countriesRepository;
        }

        public async Task<IEnumerable<TViewModel>> GetAllAsync<TViewModel>()
        {
            return await this._usersRepository.AllAsNoTracking()
                .To<TViewModel>()
                .ToListAsync();
        }

        public async Task<IEnumerable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName)
        {
            return await this._usersRepository.AllAsNoTracking().Where(u => u.Roles.Any(r => r.RoleId == this.Roles[roleName]))
                .To<TViewModel>()
                .ToListAsync();
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllRoles()
        {
            return this.Roles;
        }

        public async Task<IEnumerable<string>> GetAllCountries()
        {
            return await this._countriesRepository
                .AllAsNoTrackingWithDeleted()
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToListAsync();
        }
    }
}
