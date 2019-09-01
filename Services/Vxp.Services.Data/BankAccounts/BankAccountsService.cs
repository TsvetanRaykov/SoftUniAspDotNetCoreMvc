namespace Vxp.Services.Data.BankAccounts
{
    using System;
    using Mapping;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;

    public class BankAccountsService : IBankAccountsService
    {
        private readonly IDeletableEntityRepository<BankAccount> _bankAccountsRepository;

        public BankAccountsService(IDeletableEntityRepository<BankAccount> bankAccountsRepository)
        {
            this._bankAccountsRepository = bankAccountsRepository;
        }

        public async Task<TViewModel> CreateBankAccountAsync<TViewModel>(TViewModel bankAccount)
        {
            var appBankAccount = AutoMapper.Mapper.Map<BankAccount>(bankAccount);

            await this._bankAccountsRepository.AddAsync(appBankAccount);
            await this._bankAccountsRepository.SaveChangesAsync();

            bankAccount = AutoMapper.Mapper.Map<TViewModel>(appBankAccount);
            return bankAccount;
        }

        public TViewModel GetBankAccountById<TViewModel>(int bankAccountId)
        {
            var bankAccount = this._bankAccountsRepository.AllAsNoTracking()
                .FirstOrDefault(b => b.Id == bankAccountId);

            if (bankAccount == null)
            {
                return default;
            }

            return AutoMapper.Mapper.Map<TViewModel>(bankAccount);
        }

        public IQueryable<TViewModel> GetBankAccountsForUser<TViewModel>(string userName)
        {
            var bankAccounts = this._bankAccountsRepository.AllAsNoTracking()
                .Where(b => b.Owner.UserName == userName)
                .To<TViewModel>();
            return bankAccounts;
        }

        public async Task<bool> RemoveBankAccountAsync(int bankAccountId)
        {
            var bankAccount = await this._bankAccountsRepository.GetByIdWithDeletedAsync(bankAccountId);
            if (bankAccount == null)
            {
                return false;
            }

            this._bankAccountsRepository.Delete(bankAccount);
            await this._bankAccountsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignDistributorKeyToBankAccountAsync(int bankAccountId, string distributorKey)
        {
            var bankAccountFromDb = await this._bankAccountsRepository.GetByIdWithDeletedAsync(bankAccountId);

            if (bankAccountFromDb == null)
            {
                return false;
            }

            bankAccountFromDb.DistributorKeys.Add(new DistributorKey
            {
                KeyCode = new Guid(distributorKey)
            });

            await this._bankAccountsRepository.SaveChangesAsync();
            return true;
        }

        public async Task UpdateBankAccountAsync<TViewModel>(TViewModel bankAccount)
        {
            var appBankAccount = AutoMapper.Mapper.Map<BankAccount>(bankAccount);

            var bankAccountFromDb = await this._bankAccountsRepository.GetByIdWithDeletedAsync(appBankAccount.Id);

            AutoMapper.Mapper.Map(bankAccount, bankAccountFromDb);

         //   this._bankAccountsRepository.Update(appBankAccount);
            await this._bankAccountsRepository.SaveChangesAsync();

        }
    }
}