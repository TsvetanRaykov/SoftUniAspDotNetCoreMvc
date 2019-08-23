using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Services.Data.Products;
using Vxp.Web.ViewModels.Customers.Products;

namespace Vxp.Web.Areas.Customer.Controllers
{
    public class ProductsController : CustomersController
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

        public IActionResult View(int id)
        {
            var product = this._productsService.GetAllProducts<ProductViewModel>().FirstOrDefault(p => p.Id == id);

            product?.Images.Add(product.Image);

            return this.View(product);
        }
    }
}