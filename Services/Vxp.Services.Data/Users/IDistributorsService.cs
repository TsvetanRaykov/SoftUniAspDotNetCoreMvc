using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vxp.Data.Models;

namespace Vxp.Services.Data.Users
{
    public interface IDistributorsService
    {
        Task<string> GenerateNewDistributorKeyAsync(string distributorName);

        Task<bool> AddCustomerToDistributor(string customerName, string distributorKey);

        Task<IEnumerable<TViewModel>> GetCustomers<TViewModel>(string distributorName);

    }
}