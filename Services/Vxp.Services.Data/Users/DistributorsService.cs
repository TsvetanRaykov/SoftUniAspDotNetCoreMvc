using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Vxp.Common;
using Vxp.Data.Common.Repositories;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Services.Data.Users
{
    public class DistributorsService : IDistributorsService
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IDeletableEntityRepository<ApplicationUser> _usersRepository;
        private readonly IRepository<DistributorUser> _distributorUsersRepository;
        private readonly IRepository<DistributorKey> _distributorKeysRepository;

        public DistributorsService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IRepository<DistributorUser> distributorUsersRepository,
            IRepository<DistributorKey> distributorKeysRepository,
            UserManager<ApplicationUser> userManager)
        {
            this._usersRepository = usersRepository;
            this._distributorUsersRepository = distributorUsersRepository;
            this._distributorKeysRepository = distributorKeysRepository;
            this._userManager = userManager;
        }

        public async Task<string> GenerateNewDistributorKeyAsync(string distributorName)
        {
            var distributor = this._usersRepository.All()
                .Include(u => u.BankAccounts)
                .ThenInclude(ba => ba.DistributorKeys)
                .FirstOrDefault(u => u.UserName == distributorName);

            var newDistributorKey = new DistributorKey
            {
                KeyCode = Guid.NewGuid(),
                BankAccount = distributor?.BankAccounts.First()
            };

            distributor?.BankAccounts.FirstOrDefault()?.DistributorKeys.Add(newDistributorKey);
            await this._usersRepository.SaveChangesAsync();

            return newDistributorKey.KeyCode.ToString();
        }

        public async Task<bool> AddCustomerToDistributorAsync(string customerName, string distributorKey)
        {
            var customerFromDb = await this._usersRepository
                .All()
                .Include(u => u.Roles).ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u => u.UserName == customerName);

            var distributorKeyFromDb = await this._usersRepository.All()
                .SelectMany(u => u.BankAccounts
                    .SelectMany(ba => ba.DistributorKeys
                       .Where(dk => dk.KeyCode.ToString() == distributorKey)))
                .SingleOrDefaultAsync();

            if (customerFromDb == null || distributorKeyFromDb == null)
            {
                return false;
            }

            var newRelation = new DistributorUser
            {
                ApplicationUserId = customerFromDb.Id,
                DistributorKeyId = distributorKeyFromDb.Id
            };

            customerFromDb.Distributors.Add(newRelation);
            distributorKeyFromDb.Customers.Add(newRelation);

            if (!await this._userManager.IsInRoleAsync(customerFromDb, GlobalConstants.Roles.CustomerRoleName))
            {
                var roles = await this._userManager.GetRolesAsync(customerFromDb);
                await this._userManager.RemoveFromRolesAsync(customerFromDb, roles.ToArray());
                await this._userManager.AddToRoleAsync(customerFromDb, GlobalConstants.Roles.CustomerRoleName);
            }

            await this._usersRepository.SaveChangesAsync();
            return true;
        }

        public Task<IQueryable<TViewModel>> GetCustomers<TViewModel>(string distributorName)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<TViewModel>> GetDistributorsForUser<TViewModel>(string customerName)
        {

            var distributors = this._usersRepository.AllAsNoTracking()
                .Include(u => u.BankAccounts)
                .Include(u => u.Company)
                .Include(u => u.ContactAddress)
                .Where(u =>
                    u.BankAccounts.Any(ba =>
                        ba.DistributorKeys.Any(dk =>
                            dk.Customers.Any(du => du.ApplicationUser.UserName == customerName))));

            return Task.Run(() => distributors.To<TViewModel>());
        }

        public async Task<bool> RemoveCustomerFromDistributorAsync(string customerName, string distributorName)
        {
            var distributorKey = this._distributorKeysRepository.All()
                .Include(dk => dk.Customers)
                .FirstOrDefault(dk => dk.Customers.Any(c => c.ApplicationUser.UserName == customerName) && dk.BankAccount.Owner.UserName == distributorName);

            var relations = distributorKey?.Customers.ToList();

            for (var i = 0; i < relations?.Count; i++)
            {
                this._distributorUsersRepository.Delete(relations[i]);
            }

            // TODO: Reconsider for multiple customers to single distributor key approach
            this._distributorKeysRepository.Delete(distributorKey);

            await this._distributorKeysRepository.SaveChangesAsync();

            return true;
        }
    }
}