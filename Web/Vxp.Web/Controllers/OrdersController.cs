using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Vxp.Services.Data.Orders;
using Vxp.Services.Data.Products;
using Vxp.Services.Data.Projects;
using Vxp.Services.Data.Users;
using Vxp.Web.Infrastructure.Extensions;
using Vxp.Web.ViewModels.Orders;

namespace Vxp.Web.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProductToOrder(int productId)
        {

            var currentOrder = this.HttpContext.Session.Get<List<int>>("order") ?? new List<int>();

            currentOrder.Add(productId);
            this.HttpContext.Session.Set("order", currentOrder);

            return this.Ok(currentOrder.Count);
        }
        

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> OrderNew(OrderInputModel inputModel)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        if (inputModel.Create)
        //        {
        //            inputModel.Products.RemoveAll(p => p.Quantity == 0);

        //            foreach (var product in inputModel.Products)
        //            {
        //                var priceModifierData = new Dictionary<string, string>
        //                    {
        //                        {nameof(product.PriceModifierType), product.PriceModifierType.ToString()},
        //                        {
        //                            nameof(product.ModifierValue),
        //                            product.ModifierValue.ToString(CultureInfo.InvariantCulture)
        //                        }
        //                    };
        //                product.PriceModifierData = priceModifierData.ToJson();
        //            }

        //            if (inputModel.Products.Count > 0)
        //            {
        //                await this._ordersService.CreateOrderAsync(inputModel,
        //                    this.User.FindFirstValue(ClaimTypes.NameIdentifier));
        //            }

        //            return this.OrderNewRemove();
        //        }
        //        var updatedOrder = new List<int>();
        //        foreach (var product in inputModel.Products)
        //        {
        //            for (int i = 0; i < product.Quantity; i++)
        //            {
        //                updatedOrder.Add(product.ProductId);
        //            }
        //        }
        //        this.HttpContext.Session.Set("order", updatedOrder);
        //    }

        //    return this.RedirectToAction(nameof(this.OrderNew));

        //}

        public IActionResult OrderNewRemove()
        {
            this.HttpContext.Session.Remove("order");
            return this.RedirectToAction("Index", new { controller = "Products" });
        }

    }
}