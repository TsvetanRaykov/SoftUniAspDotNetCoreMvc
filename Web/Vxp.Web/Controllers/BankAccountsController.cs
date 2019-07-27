using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Vxp.Services.Data.BankAccounts;
using Vxp.Web.ViewModels.Components;

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
                .GetAllBankAccounts<EditUserProfileViewComponentModelBankAccountModel>()
                .FirstOrDefault(x => x.Id == id);

            return new JsonResult(bankAccount);
        }
    }
}