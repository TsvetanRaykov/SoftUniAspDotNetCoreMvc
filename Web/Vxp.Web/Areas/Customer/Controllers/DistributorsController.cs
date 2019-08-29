using System;
using Vxp.Services.Models;

namespace Vxp.Web.Areas.Customer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Vxp.Services.Data.Users;
    using Vxp.Web.ViewModels.Customer.Distributors;

    public class DistributorsController : CustomersController
    {
        private readonly IDistributorsService _distributorsService;

        public DistributorsController(IDistributorsService distributorsService)
        {
            this._distributorsService = distributorsService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DistributorsListViewModel
            {
                Distributors = await this._distributorsService.GetDistributorsForUserAsync<DistributorListViewModel>(this.User.Identity.Name)
                    .GetAwaiter().GetResult().ToListAsync(),
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Distributor(string id)
        {
            var distributors = await
                this._distributorsService.GetDistributorsForUserAsync<DistributorViewModelDetails>(this.User.Identity.Name);

            var viewModel = new DistributorViewModel
            {
                Details = await distributors.FirstOrDefaultAsync(u => u.Id == id)
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm(Name = "DistributorKeyRegisterInputModel")] DistributorKeyRegisterInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                await this._distributorsService.AddCustomerToDistributorAsync(this.User.Identity.Name, inputModel.DistributorKey);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [AcceptVerbs("Post")]
        public async Task<IActionResult> ValidateDistributorKey([FromForm(Name = "DistributorKeyRegisterInputModel.DistributorKey")]string key)
        {
            if (!Guid.TryParse(key, out var keyGuid))
            {
                return this.Json("Invalid distributor key.");
            }

            var distributor = await this._distributorsService.GetDistributorByKey<UserDto>(keyGuid);
            if (distributor == null)
            {
                return this.Json("Distributor not found.");
            }



            return this.Json(true);
        }
    }
}