using System;
using System.Linq;
using System.Linq.Expressions;
using Vxp.Data.Models;
using Vxp.Web.ViewModels.Users;

namespace Vxp.Services.Data.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<IQueryable<TViewModel>> GetAll<TViewModel>(Expression<Func<ApplicationUser, bool>> exp = null);
        Task<IQueryable<TViewModel>> GetAllWithDeleted<TViewModel>(Expression<Func<ApplicationUser, bool>> exp = null);
        Task<IQueryable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName);
        Task<string> CreateUserAsync<TViewModel>(TViewModel userModel, string password, string role);
        Task<bool> UpdateUserAsync<TViewModel>(TViewModel userModel, IEnumerable<string> roleNames);
        Task<bool> UpdateUserPasswordAsync(string userId, string password);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> RestoreUserAsync(string userId);
        Task<bool> IsRegistered(string userName);
        Task PopulateCommonUserModelPropertiesAsync(UserProfileInputModel userProfile);
    }
}