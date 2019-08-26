using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vxp.Services.Data.Products;
using Vxp.Web.ViewModels.Orders;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using Infrastructure.Extensions;

    public class OrdersController : DistributorsController
    {
        private readonly IProductsService _productsService;

        public OrdersController(IProductsService productsService)
        {
            this._productsService = productsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OrderNew()
        {

            var currentOrder = this.HttpContext.Session.Get<List<int>>("order") ?? new List<int>();

            var viewModel = new OrderInputModel
            {
                Products = await this._productsService.GetAllProducts<OrderProductViewModel>()
                    .Where(p => currentOrder.Contains(p.Id)).ToListAsync()
            };

            return this.View(viewModel);
        }
    }
}