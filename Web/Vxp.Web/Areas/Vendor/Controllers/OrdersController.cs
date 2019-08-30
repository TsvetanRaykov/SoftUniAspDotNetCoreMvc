namespace Vxp.Web.Areas.Vendor.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Services.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using ViewModels.Orders;
    using Vxp.Data.Common.Enums;
    using Vxp.Services.Data.Orders;
    using Vxp.Services.Data.Users;

    public class OrdersController : VendorsController
    {
        private readonly IUsersService _usersService;
        private readonly IOrdersService _ordersService;

        public OrdersController(
            IUsersService usersService,
            IOrdersService ordersService)
        {
            this._usersService = usersService;
            this._ordersService = ordersService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(OrderEditInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                await this._ordersService.UpdateOrderStatusAsync(inputModel.Id, inputModel.Status);
            }
            return this.Redirect(inputModel.ReturnUrl);
        }
    }
}