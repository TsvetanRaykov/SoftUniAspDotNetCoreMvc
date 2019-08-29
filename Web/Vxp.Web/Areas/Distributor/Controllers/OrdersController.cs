using Vxp.Data.Common.Enums;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Vxp.Services.Data.Products;
    using Vxp.Services.Data.Users;
    using Services.Models;
    using ViewModels.Orders;
    using ViewModels.Products;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Claims;
    using Vxp.Services.Data.Orders;
    using Vxp.Services.Data.Projects;

    public class OrdersController : DistributorsController
    {
        private readonly IProductsService _productsService;
        private readonly IProductPricesService _productPricesService;
        private readonly IUsersService _usersService;
        private readonly IProjectsService _projectsService;
        private readonly IOrdersService _ordersService;

        public OrdersController(
            IProductsService productsService,
            IProductPricesService productPricesService,
            IUsersService usersService,
            IProjectsService projectsService,
            IOrdersService ordersService)
        {
            this._productsService = productsService;
            this._productPricesService = productPricesService;
            this._usersService = usersService;
            this._projectsService = projectsService;
            this._ordersService = ordersService;
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

            viewModel.Seller = await this._usersService
                .GetAllInRoleAsync<ProductSellerViewModel>(GlobalConstants.Roles.VendorRoleName)
                .GetAwaiter().GetResult().FirstOrDefaultAsync();

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

                    return this.OrderNewRemove();
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

            return this.RedirectToAction(nameof(this.OrderNew));

        }

        public IActionResult OrderNewRemove()
        {
            this.HttpContext.Session.Remove("order");
            return this.RedirectToAction("Index", new { controller = "Products" });
        }

    }
}