namespace Vxp.Services.Data.BankAccounts
{
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using Mapping;

    public class BankAccountsService : IBankAccountsService
    {
        private readonly IDeletableEntityRepository<BankAccount> _bankAccountsRepository;

        public BankAccountsService(IDeletableEntityRepository<BankAccount> bankAccountsRepository)
        {
            this._bankAccountsRepository = bankAccountsRepository;
        }

        public async Task<TViewModel> CreateBankAccount<TViewModel>(TViewModel bankAccount)
        {
            var appBankAccount = AutoMapper.Mapper.Map<BankAccount>(bankAccount);

            await this._bankAccountsRepository.AddAsync(appBankAccount);
            await this._bankAccountsRepository.SaveChangesAsync();

            bankAccount = AutoMapper.Mapper.Map<TViewModel>(appBankAccount);
            return bankAccount;
        }

        public IQueryable<TViewModel> GetAllBankAccounts<TViewModel>()
        {
            return this._bankAccountsRepository.AllAsNoTracking().To<TViewModel>();
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

        public async Task UpdateBankAccount<TViewModel>(TViewModel bankAccount)
        {
            var appBankAccount = AutoMapper.Mapper.Map<BankAccount>(bankAccount);
            this._bankAccountsRepository.Update(appBankAccount);
            await this._bankAccountsRepository.SaveChangesAsync();

        }
    }
}