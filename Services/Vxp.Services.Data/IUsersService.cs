namespace Vxp.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Vxp.Services.Models.Administration.Users;

    public interface IUsersService
    {
        Task<IEnumerable<TViewModel>> GetAllAsync<TViewModel>();

        Task<IEnumerable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName);

        Task<IEnumerable<string>> GetAllCountries();

        Task<string> CreateUser<TViewModel>(TViewModel userModel);
    }
}