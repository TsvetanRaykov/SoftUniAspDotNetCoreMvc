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
        Task<IQueryable<TViewModel>> GetAll<TViewModel>(Expression<Func<ApplicationUser, bool>> exp = null);

        Task<IQueryable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName);

        //TODO: Move to AddressesService or so
        Task<IEnumerable<string>> GetAllCountriesAsync();

        Task<string> CreateUser<TViewModel>(TViewModel userModel);

        bool UpdateUser<TViewModel>(TViewModel userModel);

        Task<bool> UpdateUserPasswordAsync(string userId, string password);
    }
}