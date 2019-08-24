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
            var distributors = await this._distributorsService.GetDistributorsForUserAsync<DistributorsListViewModel>(this.User.Identity.Name);
            var viewModel = await distributors.ToListAsync();

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
    }
}