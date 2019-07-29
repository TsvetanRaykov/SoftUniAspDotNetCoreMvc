using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vxp.Data.Common.Repositories;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Services.Data.Users
{
    public class DistributorsService : IDistributorsService
    {
        //private readonly IRepository<DistributorUser> _distributorUsersRepository;
        //private readonly IDeletableEntityRepository<DistributorKey> _distributorKeysRepository;
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDeletableEntityRepository<ApplicationUser> _usersRepository;

        public DistributorsService(IDeletableEntityRepository<ApplicationUser> usersRepository
            //IRepository<DistributorUser> distributorUsersRepository,
            //UserManager<ApplicationUser> userManager,
            //IDeletableEntityRepository<DistributorKey> distributorKeysRepository,
            )
        {
            this._usersRepository = usersRepository;
            //this._distributorUsersRepository = distributorUsersRepository;
            //this._userManager = userManager;
            //this._distributorKeysRepository = distributorKeysRepository;
            //this._applicationUsersRepository = applicationUsersRepository;
        }

        public async Task<string> GenerateNewDistributorKeyAsync(string distributorName)
        {
            //var distributor = this._userManager.Users
            //    .Include(u => u.BankAccounts)
            //    .FirstOrDefault(u => u.UserName == distributorName);

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
            //await this._distributorKeysRepository.AddAsync(newDistributorKey);
            //await this._distributorKeysRepository.SaveChangesAsync();

            return newDistributorKey.KeyCode.ToString();
        }

        public async Task<bool> AddCustomerToDistributor(string customerName, string distributorKey)
        {
            var customerFromDb = await this._usersRepository.AllAsNoTracking().FirstOrDefaultAsync(u => u.UserName == customerName);

            var distributorKeyFromDb = await this._usersRepository.All()
                .SelectMany(u => u.BankAccounts
                    .SelectMany(ba => ba.DistributorKeys
                       .Where(dk => dk.KeyCode.ToString() == distributorKey)))
                .SingleOrDefaultAsync();


            //var distributorKeyFromDb = this._distributorKeysRepository.AllAsNoTracking()
            //    .FirstOrDefault(k => k.KeyCode.ToString() == distributorKey);

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

            await this._usersRepository.SaveChangesAsync();

            //await this._distributorUsersRepository.AddAsync(newRelation);
            //await this._distributorUsersRepository.SaveChangesAsync();

            return true;
        }

        public Task<IQueryable<TViewModel>> GetCustomers<TViewModel>(string distributorName)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<TViewModel>> GetDistributors<TViewModel>(string customerName)
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

        public Task<IQueryable<TViewModel>> GetAllDistributors<TViewModel>()
        {
            throw new NotImplementedException();
        }
    }
}