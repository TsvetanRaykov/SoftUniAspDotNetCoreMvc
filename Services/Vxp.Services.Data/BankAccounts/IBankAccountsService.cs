namespace Vxp.Services.Data.BankAccounts
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface IBankAccountsService
    {
        Task<TViewModel> CreateBankAccountAsync<TViewModel>(TViewModel bankAccount);
        TViewModel GetBankAccountById<TViewModel>(int bankAccountId);
        IQueryable<TViewModel> GetBankAccountsForUser<TViewModel>(string userName);
        Task<bool> RemoveBankAccountAsync(int bankAccountId);
        Task UpdateBankAccountAsync<TViewModel>(TViewModel bankAccount);
        Task<bool> AssignDistributorKeyToBankAccountAsync(int bankAccountId, string distributorKey);
    }
}