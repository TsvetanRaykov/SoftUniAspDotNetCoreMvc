using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vxp.Data.Models;

namespace Vxp.Services.Data.BankAccounts
{
    public interface IBankAccountsService
    {
        Task<TViewModel> CreateBankAccount<TViewModel>(TViewModel bankAccount);
        IQueryable<TViewModel> GetAllBankAccounts<TViewModel>();
        Task<bool> RemoveBankAccountAsync(int bankAccountId);
        Task<bool> UpdateBankAccount<TViewModel>(TViewModel bankAccount);
    }
}