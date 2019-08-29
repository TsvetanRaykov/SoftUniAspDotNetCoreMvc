namespace Vxp.Services.Data.Users
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using System;

    public interface IDistributorsService
    {
        Task<bool> AddCustomerToDistributorAsync(string customerName, string distributorKey);

        Task<bool> RemoveCustomerFromDistributorAsync(string customerName, string distributorName);

        Task<IQueryable<TViewModel>> GetCustomersAsync<TViewModel>(string distributorName);

        Task<List<TViewModel>> GetCustomerInvitationsAsync<TViewModel>(string senderId);

        Task<TViewModel> GetDistributorByKey<TViewModel>(Guid distributorKey);

        Task<IQueryable<TViewModel>> GetDistributorsForUserAsync<TViewModel>(string customerName);

        Task<bool> SendInvitationToCustomerAsync(EmailDto email, string sederId);

        string GenerateNewKeyForDistributor(string distributorName);

    }
}