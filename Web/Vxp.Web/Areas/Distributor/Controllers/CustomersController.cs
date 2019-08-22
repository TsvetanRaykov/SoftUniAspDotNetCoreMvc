using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vxp.Services.Data.BankAccounts;
using Vxp.Services.Data.Users;
using Vxp.Services.Models;
using Vxp.Web.ViewModels.Distributor.Customers;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    public class CustomersController : DistributorsController
    {
        private readonly IDistributorsService _distributorsService;
        private readonly IBankAccountsService _bankAccountsService;

        public CustomersController(
            IDistributorsService distributorsService,
            IBankAccountsService bankAccountsService)
        {
            this._distributorsService = distributorsService;
            this._bankAccountsService = bankAccountsService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Invite()
        {
            var viewModel = new CustomersInvitationInputModel
            {
                DistributorKey = this._distributorsService.GenerateNewKeyForDistributor(this.User.Identity.Name)?.ToString(),
                SentInvitations = await this._distributorsService
                    .GetCustomerInvitationsAsync<CustomersInvitationInputModel>(
                        this.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                AvailableBankAccounts = this._bankAccountsService.GetBankAccountsForUser<SelectListItem>(this.User.Identity.Name).ToList()
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(CustomersInvitationInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                var serviceModel = AutoMapper.Mapper.Map<EmailDto>(inputModel);

                var uriBuilder = new UriBuilder
                {
                    Scheme = this.HttpContext.Request.Scheme,
                    Host = this.HttpContext.Request.Host.Host,
                    Port = this.HttpContext.Request.Host.Port.GetValueOrDefault(80),
                    Path = "/Identity/Account/Register/",
                    Query = $"key={inputModel.DistributorKey}"
                };
                var registerUrl = uriBuilder.ToString();

                serviceModel.MessageBody += $"<br/>You can register by <a href='{HtmlEncoder.Default.Encode(registerUrl)}'>clicking here</a>.";
                serviceModel.MessageBody += $"<h4>{serviceModel.DistributorKey}</h4>";

                if (await this._bankAccountsService.AssignDistributorKeyToBankAccountAsync(inputModel.BankAccountId, inputModel.DistributorKey))
                {
                    if (await this._distributorsService.SendInvitationToCustomerAsync(serviceModel,
                        this.User.FindFirstValue(ClaimTypes.NameIdentifier)))
                    {
                        this.TempData["SuccessMessage"] = "Invitation sent.";
                    }
                }
                else
                {
                    this.TempData["ErrorMessage"] = "Invitation cannot be send.";
                }
            }

            return this.RedirectToAction(nameof(Invite));
        }


        public IActionResult Pending()
        {
            return this.View();
        }
    }
}