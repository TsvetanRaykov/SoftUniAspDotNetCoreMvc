using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Services.Data.BankAccounts;
using Vxp.Web.ViewModels.Users;

namespace Vxp.Web.Controllers
{

    public class BankAccountsController : ApiController
    {
        private readonly IBankAccountsService _bankAccountsService;

        public BankAccountsController(IBankAccountsService bankAccountsService)
        {
            this._bankAccountsService = bankAccountsService;
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var bankAccount = this._bankAccountsService
                .GetAllBankAccounts<UserProfileBankAccountViewModel>()
                .FirstOrDefault(x => x.Id == id);
            if (bankAccount == null)
            {
                return this.NoContent();
            }

            return this.Ok(bankAccount);
        }

        [HttpPut]
        public async Task<ActionResult> Update(UserProfileBankAccountViewModel bankAccountModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }
            await this._bankAccountsService.UpdateBankAccount(bankAccountModel);

            return this.Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserProfileBankAccountViewModel bankAccountModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var newBankAccount = await this._bankAccountsService.CreateBankAccount(bankAccountModel);

            if (newBankAccount.Id > 0)
            {
                return this.Ok();
            }

            return this.BadRequest();
        }

        [HttpDelete("{id}")]
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