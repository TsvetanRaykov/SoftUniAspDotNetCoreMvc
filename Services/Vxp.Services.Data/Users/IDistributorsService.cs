using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Users
{
    public interface IDistributorsService
    {
        Task<string> GenerateNewDistributorKeyAsync(string distributorName);

        Task<bool> AddCustomerToDistributor(string customerName, string distributorKey);

        Task<IQueryable<TViewModel>> GetCustomers<TViewModel>(string distributorName);

        Task<IQueryable<TViewModel>> GetDistributors<TViewModel>(string customerName);

        Task<IQueryable<TViewModel>> GetAllDistributors<TViewModel>();

    }
}