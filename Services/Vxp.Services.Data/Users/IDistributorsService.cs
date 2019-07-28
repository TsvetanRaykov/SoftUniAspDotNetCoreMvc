using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Users
{
    public interface IDistributorsService
    {
        Task<string> GenerateNewDistributorKeyAsync(string distributorName);

        Task<bool> AddCustomerToDistributor(string customerName, string distributorKey);

        Task<IEnumerable<TViewModel>> GetCustomers<TViewModel>(string distributorName);

    }
}