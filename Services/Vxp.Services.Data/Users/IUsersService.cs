namespace Vxp.Services.Data.Users
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<IEnumerable<TViewModel>> GetAllAsync<TViewModel>();

        Task<IEnumerable<TViewModel>> GetAllInRoleAsync<TViewModel>(string roleName);

        Task<IEnumerable<string>> GetAllCountries();

        Task<string> CreateUser<TViewModel>(TViewModel userModel);
    }
}