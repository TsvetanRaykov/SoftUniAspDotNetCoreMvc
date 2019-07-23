using System.Linq;
using Microsoft.AspNetCore.Identity;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Services.Data.Users
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesService(RoleManager<ApplicationRole> roleManager)
        {
            this._roleManager = roleManager;
        }

        public IQueryable<TViewModel> GetAll<TViewModel>()
        {
            return this._roleManager.Roles.To<TViewModel>();
        }

        public int Count()
        {
            return this._roleManager.Roles.Count();
        }
    }
}