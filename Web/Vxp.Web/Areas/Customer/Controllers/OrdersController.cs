using Vxp.Data.Common.Enums;

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
    using ViewModels.Products;
    using Vxp.Services.Data.Orders;
    using Vxp.Services.Data.Products;
    using Vxp.Services.Data.Projects;
    using Vxp.Services.Data.Users;

    public class OrdersController : CustomersController
    {
        private readonly IProductsService _productsService;
        private readonly IProjectsService _projectsService;
        private readonly IProductPricesService _productPricesService;
        private readonly IOrdersService _ordersService;
        private readonly IDistributorsService _distributorsService;

        public OrdersController(
            IProductsService productsService,
            IProjectsService projectsService,
            IProductPricesService productPricesService,
            IOrdersService ordersService,
            IDistributorsService distributorsService)
        {
            this._productsService = productsService;
            this._projectsService = projectsService;
            this._productPricesService = productPricesService;
            this._ordersService = ordersService;
            this._distributorsService = distributorsService;
        }

        public async Task<IActionResult> OrderNew()
        {
            var currentOrder = this.HttpContext.Session.Get<List<int>>("order") ?? new List<int>();
            var viewModel = new OrderInputModel
            {
                Products = await this._productsService.GetAllProducts<OrderProductViewModel>()
                    .Where(p => currentOrder.Contains(p.ProductId)).ToListAsync(),
                Sellers = await this._distributorsService.GetDistributorsForUserAsync<ProductSellerViewModel>(this.User.Identity.Name)
                    .GetAwaiter().GetResult().ToListAsync(),
                AvailableProjects = await this._projectsService.GetAllProjects<OrderProjectViewModel>(this.User.Identity.Name).ToListAsync()
            };

            if (this.TempData.ContainsKey("SellerId"))
            {
                viewModel.SellerId = this.TempData["SellerId"] as string;
            }
            var seller = viewModel.Sellers.FirstOrDefault(s => s.Id == viewModel.SellerId) ?? viewModel.Sellers.First();
            viewModel.SellerId = seller.Id;

            var priceModifier = await
                                    this._productPricesService.GetBuyerPriceModifiers<PriceModifierDto>(this.User.Identity.Name)
                                        .Where(pm => pm.SellerId == seller.Id)
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

            this.TempData["SellerId"] = inputModel.SellerId;

            return this.RedirectToAction(nameof(this.OrderNew));

        }

        public IActionResult OrderNewRemove()
        {
            this.HttpContext.Session.Remove("order");
            return this.RedirectToAction("Index", new { controller = "Products" });
        }

    }
}