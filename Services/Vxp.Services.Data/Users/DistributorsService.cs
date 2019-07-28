using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vxp.Data.Common.Repositories;
using Vxp.Data.Models;

namespace Vxp.Services.Data.Users
{
    public class DistributorsService : IDistributorsService
    {
        private readonly IRepository<DistributorUser> _distributorUsersRepository;
        private readonly IDeletableEntityRepository<DistributorKey> _distributorKeysRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DistributorsService(
            IRepository<DistributorUser> distributorUsersRepository,
            UserManager<ApplicationUser> userManager,
            IDeletableEntityRepository<DistributorKey> distributorKeysRepository)
        {
            this._distributorUsersRepository = distributorUsersRepository;
            this._userManager = userManager;
            this._distributorKeysRepository = distributorKeysRepository;
        }

        public async Task<string> GenerateNewDistributorKeyAsync(string distributorName)
        {
            var distributor = this._userManager.Users
                .Include(u => u.BankAccounts)
                .FirstOrDefault(u => u.UserName == distributorName);

            var newDistributorKey = new DistributorKey
            {
                KeyCode = Guid.NewGuid(),
                BankAccount = distributor?.BankAccounts.First()
            };

            await this._distributorKeysRepository.AddAsync(newDistributorKey);
            await this._distributorKeysRepository.SaveChangesAsync();

            return newDistributorKey.KeyCode.ToString();
        }

        public async Task<bool> AddCustomerToDistributor(string customerName, string distributorKey)
        {
            var customerFromDb = await this._userManager.FindByNameAsync(customerName);
            var distributorKeyFromDb = this._distributorKeysRepository.AllAsNoTracking()
                .FirstOrDefault(k => k.KeyCode.ToString() == distributorKey);

            if (customerFromDb == null || distributorKeyFromDb == null)
            {
                return false;
            }

            var newRelation = new DistributorUser
            {
                ApplicationUserId = customerFromDb.Id,
                DistributorKeyId = distributorKeyFromDb.Id
            };

            await this._distributorUsersRepository.AddAsync(newRelation);
            await this._distributorUsersRepository.SaveChangesAsync();

            return true;
        }

        public Task<IEnumerable<TViewModel>> GetCustomers<TViewModel>(string distributorName)
        {
            throw new System.NotImplementedException();
        }
    }
}