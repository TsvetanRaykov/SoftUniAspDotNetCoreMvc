using System;
using System.Linq;
using System.Linq.Expressions;
using Vxp.Data.Models;

namespace Vxp.Services.Data.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        IQueryable<TViewModel> GetAll<TViewModel>(Expression<Func<ApplicationUser, bool>> exp = null);

        Task<IEnumerable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName);

        Task<IEnumerable<string>> GetAllCountriesAsync();

        Task<string> CreateUser<TViewModel>(TViewModel userModel);
    }
}