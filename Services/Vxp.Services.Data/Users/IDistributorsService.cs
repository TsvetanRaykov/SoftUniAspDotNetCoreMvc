using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Users
{
    public interface IDistributorsService
    {
        Task<string> GenerateNewDistributorKeyAsync(string distributorName);

        Task<bool> AddCustomerToDistributorAsync(string customerName, string distributorKey);

        Task<bool> RemoveCustomerFromDistributorAsync(string customerName, string distributorName);

        Task<IQueryable<TViewModel>> GetCustomers<TViewModel>(string distributorName);

        Task<IQueryable<TViewModel>> GetDistributorsForUser<TViewModel>(string customerName);
    }
}