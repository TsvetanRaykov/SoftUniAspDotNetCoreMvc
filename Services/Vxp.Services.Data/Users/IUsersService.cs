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
        Task<IQueryable<TViewModel>> GetAllWithDeleted<TViewModel>(Expression<Func<ApplicationUser, bool>> exp = null);
        Task<IQueryable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName);
        Task<string> CreateUser<TViewModel>(TViewModel userModel, string password, string role);
        Task<bool> UpdateUser<TViewModel>(TViewModel userModel, IEnumerable<string> roleNames);
        Task<bool> UpdateUserPasswordAsync(string userId, string password);
        Task<bool> DeleteUser(string userId);
        Task<bool> RestoreUser(string userId);
        Task<bool> IsRegistered(string userName);
    }
}