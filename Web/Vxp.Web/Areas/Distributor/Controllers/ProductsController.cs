using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index(int categoryId)
        {
            var products = this._productsService.GetAllProducts<ProductListViewModel>().Where(p => p.IsAvailable);
            if (categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            var viewModel = new ProductsListViewModel
            {
                CategoryFilterId = categoryId,
                Products = await products.ToListAsync()
            };

            return View(viewModel);
        }
    }
}