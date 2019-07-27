using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vxp.Data.Models;

namespace Vxp.Services.Data.BankAccounts
{
    public interface IBankAccountsService
    {
        TViewModel CreateBankAccount<TViewModel>();
        IQueryable<TViewModel> GetAllBankAccounts<TViewModel>();
        Task<bool> RemoveBankAccountAsync(int bankAccountId);
    }
}