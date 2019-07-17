using System.Linq;

namespace Vxp.Services.Data
{
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
        private Dictionary<string, string> _roles;

        private Dictionary<string, string> Roles =>
            this._roles ?? (this._roles = this._rolesRepository.AllAsNoTrackingWithDeleted()
                                                        .ToDictionaryAsync(key => key.Id, v => v.Name)
                                                        .GetAwaiter().GetResult());

        public UsersService(IDeletableEntityRepository<ApplicationUser> usersRepository, IDeletableEntityRepository<ApplicationRole> rolesRepository)
        {
            this._usersRepository = usersRepository;
            this._rolesRepository = rolesRepository;
        }

        public async Task<IEnumerable<TViewModel>> GetAllAsync<TViewModel>()
        {
            return await this._usersRepository.AllAsNoTracking()
                .To<TViewModel>()
                .ToListAsync();
        }

        public async Task<IEnumerable<TViewModel>> GetAllInRoleAsync<TViewModel>(string role)
        {
            return await this._usersRepository.AllAsNoTracking().Where(u => u.Roles.Any(r => r.RoleId == role))
                       .To<TViewModel>()
                       .ToListAsync();
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllRoles()
        {
            return this.Roles;
        }
    }
}
