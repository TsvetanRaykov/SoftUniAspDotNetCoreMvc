using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Vxp.Common;
using Vxp.Data.Common.Repositories;
using Vxp.Data.Models;
using Vxp.Services.Mapping;
using Vxp.Services.Models;

namespace Vxp.Services.Data.Users
{
    public class DistributorsService : IDistributorsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDeletableEntityRepository<ApplicationUser> _usersRepository;
        private readonly IRepository<DistributorUser> _distributorUsersRepository;
        private readonly IRepository<DistributorKey> _distributorKeysRepository;
        private readonly IDeletableEntityRepository<CustomerInvitation> _customerInvitationsRepository;
        private readonly IEmailSender _emailSender;

        public DistributorsService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IRepository<DistributorUser> distributorUsersRepository,
            IRepository<DistributorKey> distributorKeysRepository,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IDeletableEntityRepository<CustomerInvitation> customerInvitationsRepository)
        {
            this._usersRepository = usersRepository;
            this._distributorUsersRepository = distributorUsersRepository;
            this._distributorKeysRepository = distributorKeysRepository;
            this._userManager = userManager;
            this._emailSender = emailSender;
            this._customerInvitationsRepository = customerInvitationsRepository;
        }

        public string GenerateNewKeyForDistributor(string distributorName)
        {
            var distributor = this._usersRepository.All()
                .Include(u => u.BankAccounts)
                .FirstOrDefault(u => u.UserName == distributorName);

            if (distributor?.BankAccounts.FirstOrDefault() == null)
            {
                return null;
            }

            // TODO: Make the key more fancy
            return Guid.NewGuid().ToString();
        }

        public async Task<bool> AddCustomerToDistributorAsync(string customerName, string distributorKey)
        {
            var customerFromDb = await this._usersRepository.All()
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

        public Task<IQueryable<TViewModel>> GetCustomersAsync<TViewModel>(string distributorName)
        {
            var customers = this._usersRepository.AllAsNoTracking()
                .Where(u => u.Distributors.Any(d => d.DistributorKey.BankAccount.Owner.UserName == distributorName));

            return Task.Run(() => customers.To<TViewModel>());
        }

        public Task<List<TViewModel>> GetCustomerInvitationsAsync<TViewModel>(string senderId)
        {
            var invitations = this._customerInvitationsRepository.AllAsNoTracking().Where(i => i.SenderId == senderId);
            return invitations.To<TViewModel>().ToListAsync();
        }

        public async Task<TViewModel> GetDistributorByKey<TViewModel>(Guid distributorKey)
        {
            var distributor = await this._distributorKeysRepository.All()
                .Include(dk => dk.BankAccount)
                .ThenInclude(bs => bs.Owner)
                .Where(dk => dk.KeyCode == distributorKey)
                .Select(dk => dk.BankAccount.Owner).FirstOrDefaultAsync();
                
            if (distributor == null)
            {
                return default;
            }

            return AutoMapper.Mapper.Map<TViewModel>(distributor);
        }

        public Task<IQueryable<TViewModel>> GetDistributorsForUserAsync<TViewModel>(string customerName)
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

        public async Task<bool> SendInvitationToCustomerAsync(EmailDto email, string senderId)
        {
            try
            {
                await this._emailSender.SendEmailAsync(
                    email.EmailTo,
                    email.Subject,
                    email.MessageBody);
            }
            catch
            {
                return false;
            }

            var customerInvitation = AutoMapper.Mapper.Map<CustomerInvitation>(email);
            customerInvitation.SenderId = senderId;

            await this._customerInvitationsRepository.AddAsync(customerInvitation);
            await this._customerInvitationsRepository.SaveChangesAsync();

            return true;

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