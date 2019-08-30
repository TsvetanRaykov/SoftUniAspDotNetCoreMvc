using System;
using Vxp.Services.Data.Users;

namespace Vxp.Web.Areas.Customer.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Services.Models;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using ViewModels.Orders;
    using Vxp.Data.Common.Enums;
    using Vxp.Services.Data.Orders;
    using Vxp.Services.Data.Products;
    using Vxp.Services.Data.Projects;

    public class OrdersController : CustomersController
    {
        private readonly IProductsService _productsService;
        private readonly IProjectsService _projectsService;
        private readonly IProductPricesService _productPricesService;
        private readonly IOrdersService _ordersService;
        private readonly IUsersService _usersService;

        public OrdersController(
            IProductsService productsService,
            IProjectsService projectsService,
            IProductPricesService productPricesService,
            IOrdersService ordersService, 
            IUsersService usersService)
        {
            this._productsService = productsService;
            this._projectsService = projectsService;
            this._productPricesService = productPricesService;
            this._ordersService = ordersService;
            this._usersService = usersService;
        }

        public async Task<IActionResult> OrderNew()
        {
            var currentOrder = this.HttpContext.Session.Get<List<int>>("order") ?? new List<int>();
            var viewModel = new OrderInputModel
            {
                Products = await this._productsService.GetAllProducts<OrderProductViewModel>()
                    .Where(p => currentOrder.Contains(p.ProductId)).ToListAsync(),
                AvailableProjects = await this._projectsService.GetAllProjects<OrderProjectViewModel>(this.User.Identity.Name).ToListAsync()
            };

            if (this.TempData.ContainsKey("ProjectId"))
            {
                viewModel.ProjectId = (int)this.TempData["ProjectId"];
            }

            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var project = viewModel.AvailableProjects.FirstOrDefault(p => p.Id == viewModel.ProjectId) ?? viewModel.AvailableProjects.FirstOrDefault();

            if (project == null)
            {
                return this.View(viewModel);
            }

            viewModel.Seller = project.Partner.Id == currentUserId ? project.Owner : project.Partner;
            viewModel.SellerId = viewModel.Seller.Id;

            var priceModifier = await
                                    this._productPricesService.GetBuyerPriceModifiers<PriceModifierDto>(this.User.Identity.Name)
                                        .Where(pm => pm.SellerId == viewModel.SellerId)
                                        .FirstOrDefaultAsync() ?? new PriceModifierDto { PriceModifierType = PriceModifierType.Decrease };

            foreach (var product in viewModel.Products)
            {
                product.Quantity = currentOrder.Count(p => p == product.ProductId);
                product.PriceModifierType = priceModifier.PriceModifierType;
                product.ModifierValue = priceModifier.PercentValue;
            }

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderNew(OrderInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                if (inputModel.Create)
                {
                    inputModel.Products.RemoveAll(p => p.Quantity == 0);

                    foreach (var product in inputModel.Products)
                    {
                        var priceModifierData = new Dictionary<string, string>
                            {
                                {nameof(product.PriceModifierType), product.PriceModifierType.ToString()},
                                {
                                    nameof(product.ModifierValue),
                                    product.ModifierValue.ToString(CultureInfo.InvariantCulture)
                                }
                            };
                        product.PriceModifierData = priceModifierData.ToJson();
                    }

                    if (inputModel.Products.Count > 0)
                    {
                        await this._ordersService.CreateOrderAsync(inputModel,
                            this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    }
                    this.HttpContext.Session.Remove("order");
                    return this.RedirectToAction("Project", new { controller = "Projects", id = inputModel.ProjectId });
                }

                var updatedOrder = new List<int>();
                foreach (var product in inputModel.Products)
                {
                    for (int i = 0; i < product.Quantity; i++)
                    {
                        updatedOrder.Add(product.ProductId);
                    }
                }
                this.HttpContext.Session.Set("order", updatedOrder);
            }

            this.TempData["ProjectId"] = inputModel.ProjectId;

            return this.RedirectToAction(nameof(this.OrderNew));

        }

        public IActionResult OrderNewRemove()
        {
            this.HttpContext.Session.Remove("order");
            return this.RedirectToAction("Index", new { controller = "Products" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderEdit([FromForm(Name = "order")]OrderInputModel inputModel, string returnUrl)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this._usersService.GetAllWithDeletedAsync<UserDto>(u => u.Id == inputModel.BuyerId)
                    .GetAwaiter().GetResult().FirstOrDefaultAsync();

                var viewModel = await this._ordersService.GetAllOrdersAsync<OrderEditInputModel>(user?.UserName)
                    .GetAwaiter().GetResult().FirstOrDefaultAsync(o => o.Id == inputModel.Id);

                if (viewModel != null)
                {

                    foreach (var product in viewModel.Products)
                    {
                        var priceModifierData = product.PriceModifierData.ToObject<Dictionary<string, string>>();

                        Enum.TryParse(priceModifierData[nameof(product.PriceModifierType)], true,
                            out PriceModifierType modifierType);

                        decimal.TryParse(priceModifierData[nameof(product.ModifierValue)], out var modifierValue);

                        product.ModifierValue = modifierValue;
                        product.PriceModifierType = modifierType;
                    }
                    viewModel.ReturnUrl = returnUrl;
                    return this.View(viewModel);
                }
            }

            return this.NotFound();
        }

    }
}