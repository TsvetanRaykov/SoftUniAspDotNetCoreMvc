using System.Linq;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Vxp.Services.Data.Products;
    using Vxp.Web.ViewModels.Distributor.Products;

    public class ProductsController : DistributorsController
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            this._productsService = productsService;
        }

        public IActionResult Index()
        {
            var products = this._productsService.GetAllProducts<ProductListViewModel>().Where(p => p.IsAvailable);
            return View(products);
        }
    }
}