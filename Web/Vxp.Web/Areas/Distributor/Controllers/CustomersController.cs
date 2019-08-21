using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Vxp.Services.Data.Users;
using Vxp.Services.Models;
using Vxp.Web.ViewModels.Distributor.Customers;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    public class CustomersController : DistributorsController
    {
        private readonly IDistributorsService _distributorsService;

        public CustomersController(IDistributorsService distributorsService)
        {
            this._distributorsService = distributorsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Invite()
        {
            var viewModel = new CustomersInvitationInputModel
            {
                DistributorKey = await this._distributorsService.GenerateNewDistributorKeyAsync(this.User.Identity.Name),
                SentInvitations = await this._distributorsService
                    .GetCustomerInvitationsAsync<CustomersInvitationInputModel>(
                        this.User.FindFirstValue(ClaimTypes.NameIdentifier))
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
                serviceModel.MessageBody += $"<h3>{serviceModel.DistributorKey}</h3>";

                if (await this._distributorsService.SendInvitationToCustomerAsync(serviceModel,
                    this.User.FindFirstValue(ClaimTypes.NameIdentifier)))
                {
                    TempData["SuccessMessage"] = "Invitation sent.";
                    inputModel.DistributorKey =
                        await this._distributorsService.GenerateNewDistributorKeyAsync(this.User.Identity.Name);
                }
                else
                {
                    TempData["ErrorMessage"] = "Invitation cannot be send.";
                }
            }

            inputModel.SentInvitations = await this._distributorsService
                .GetCustomerInvitationsAsync<CustomersInvitationInputModel>(
                    this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            return this.View(inputModel);
        }


        public IActionResult Pending()
        {
            return this.View();
        }
    }
}