namespace Vxp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using ViewModels.Users;
    using Vxp.Services.Data.BankAccounts;

    public class BankAccountsController : ApiController
    {
        private readonly IBankAccountsService _bankAccountsService;

        public BankAccountsController(IBankAccountsService bankAccountsService)
        {
            this._bankAccountsService = bankAccountsService;
        }

        [HttpGet("{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult<string> Get(int id)
        {
            var bankAccount = this._bankAccountsService
                .GetBankAccountById<UserProfileBankAccountInputModel>(id);

            if (bankAccount == null)
            {
                return this.NoContent();
            }

            return this.Ok(bankAccount);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(UserProfileBankAccountInputModel bankAccountModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }
            await this._bankAccountsService.UpdateBankAccountAsync(bankAccountModel);

            return this.Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserProfileBankAccountInputModel bankAccountModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var newBankAccount = await this._bankAccountsService.CreateBankAccountAsync(bankAccountModel);

            if (newBankAccount.Id > 0)
            {
                return this.Ok();
            }

            return this.BadRequest();
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            if (await this._bankAccountsService.RemoveBankAccountAsync(id))
            {
                return this.Accepted(id);
            }

            return this.BadRequest();
        }

    }
}