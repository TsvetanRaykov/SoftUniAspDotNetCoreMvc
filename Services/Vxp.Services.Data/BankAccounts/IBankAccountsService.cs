using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.BankAccounts
{
    public interface IBankAccountsService
    {
        Task<TViewModel> CreateBankAccount<TViewModel>(TViewModel bankAccount);
        IQueryable<TViewModel> GetAllBankAccounts<TViewModel>();
        Task<bool> RemoveBankAccountAsync(int bankAccountId);
        Task UpdateBankAccount<TViewModel>(TViewModel bankAccount);
    }
}