namespace Vxp.Web.Infrastructure.ModelBinders
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Services.Data.BankAccounts;

    public class BankAccountToSelectListItemModelBinder : IModelBinder
    {
        private readonly IBankAccountsService _bankAccountsService;

        public BankAccountToSelectListItemModelBinder(IBankAccountsService bankAccountsService)
        {
            this._bankAccountsService = bankAccountsService;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult =
                bindingContext.ValueProvider.GetValue(bindingContext.ModelName)
                    .Select(int.Parse)
                    .ToArray();

            bindingContext.Result = ModelBindingResult.Success(
             await this._bankAccountsService
                    .GetAllBankAccounts<SelectListItem>(bankAccount => valueProviderResult.Contains(bankAccount.Id))
                    .ToListAsync());
        }
    }
}