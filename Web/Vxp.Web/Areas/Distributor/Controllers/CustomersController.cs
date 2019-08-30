using Vxp.Services.Data.Projects;
using Vxp.Web.ViewModels.Projects;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Vxp.Services.Data.BankAccounts;
    using Vxp.Services.Data.Users;
    using Services.Models;
    using Vxp.Web.ViewModels.Distributor.Customers;
    using Vxp.Services.Data.Products;
    using ViewModels.Prices;

    public class CustomersController : DistributorsController
    {
        private readonly IDistributorsService _distributorsService;
        private readonly IBankAccountsService _bankAccountsService;
        private readonly IProductPricesService _productsPricesService;
        private readonly IProjectsService _projectsService;

        public CustomersController(
            IDistributorsService distributorsService,
            IBankAccountsService bankAccountsService,
            IProductPricesService productsPricesService,
            IProjectsService projectsService)
        {
            this._distributorsService = distributorsService;
            this._bankAccountsService = bankAccountsService;
            this._productsPricesService = productsPricesService;
            this._projectsService = projectsService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await this._distributorsService.GetCustomersAsync<CustomersListViewModel>(this.User.Identity.Name);
            var viewModel = await customers.ToListAsync();

            return this.View(viewModel);
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

        [HttpGet]
        public async Task<IActionResult> Customer(string id)
        {
            var customer = await
                this._distributorsService.GetCustomersAsync<CustomerViewModelDetails>(this.User.Identity.Name)
                    .GetAwaiter().GetResult().SingleOrDefaultAsync(u => u.Id == id);

            if (customer == null)
            {
                return this.NotFound();
            }

            var sellerId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = new CustomerViewModel
            {
                Details = customer,
                PriceModifierInputModel = customer.PriceModifiers
                                              .FirstOrDefault(pm => pm.SellerId == sellerId) ?? new PriceModifierInputModel
                                              {
                                                  BuyerId = customer.Id,
                                                  SellerId = sellerId
                                              },
                ExistingProjects = await this._projectsService.GetAllProjects<ProjectInputModel>(this.User.Identity.Name)
                    .Where(p => p.OwnerId == id || p.PartnerId == id)
                    .ToListAsync()
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePriceModel(PriceModifierInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                await this._productsPricesService.SetProductPriceModifierAsync(inputModel);
            }

            return this.Redirect(inputModel.ReturnUrl);
        }

        public IActionResult Pending()
        {
            return this.View();
        }
    }
}