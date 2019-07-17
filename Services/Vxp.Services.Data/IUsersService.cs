using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vxp.Services.Data
{
    public interface IUsersService
    {
        Task<IEnumerable<TViewModel>> GetAllAsync<TViewModel>();

        Task<IEnumerable<TViewModel>> GetAllInRoleAsync<TViewModel>(string role);

        IEnumerable<KeyValuePair<string, string>> GetAllRoles();
    }
}