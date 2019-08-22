using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Services.Models;

namespace Vxp.Services.Data.Users
{
    public interface IDistributorsService
    {
        //Task<string> GenerateNewDistributorKeyAsync(string distributorName);

        Task<bool> AddCustomerToDistributorAsync(string customerName, string distributorKey);

        Task<bool> RemoveCustomerFromDistributorAsync(string customerName, string distributorName);

        Task<IQueryable<TViewModel>> GetCustomersAsync<TViewModel>(string distributorName);

        Task<List<TViewModel>> GetCustomerInvitationsAsync<TViewModel>(string senderId);

        Task<IQueryable<TViewModel>> GetDistributorsForUserAsync<TViewModel>(string customerName);

        Task<bool> SendInvitationToCustomerAsync(EmailDto email, string sederId);

        string GenerateNewKeyForDistributor(string distributorName);

    }
}