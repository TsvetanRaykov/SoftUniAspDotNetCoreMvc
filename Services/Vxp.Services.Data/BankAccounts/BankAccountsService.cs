using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vxp.Data.Common.Repositories;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Services.Data.BankAccounts
{
    public class BankAccountsService : IBankAccountsService
    {
        private readonly IDeletableEntityRepository<BankAccount> _bankAccountsRepository;

        public BankAccountsService(IDeletableEntityRepository<BankAccount> bankAccountsRepository)
        {
            this._bankAccountsRepository = bankAccountsRepository;
        }

        public TViewModel CreateBankAccount<TViewModel>()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TViewModel> GetAllBankAccounts<TViewModel>()
        {
            return this._bankAccountsRepository.AllAsNoTracking().To<TViewModel>();
        }

        public Task<bool> RemoveBankAccountAsync(int bankAccountId)
        {
            throw new NotImplementedException();
        }
    }
}