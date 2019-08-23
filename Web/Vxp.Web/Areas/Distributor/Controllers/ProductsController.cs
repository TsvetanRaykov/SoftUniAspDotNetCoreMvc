namespace Vxp.Web.Areas.Distributor.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Vxp.Services.Data.Products;
    using Vxp.Web.ViewModels.Distributor.Products;
    using Services.Models;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class ProductsController : DistributorsController
    {
        private readonly IProductsService _productsService;
        private readonly IProductPricesService _productPricesService;

        public ProductsController(
            IProductsService productsService,
            IProductPricesService productPricesService)
        {
            this._productsService = productsService;
            this._productPricesService = productPricesService;
        }

        public async Task<IActionResult> Index(int categoryId)
        {
            var priceModifier =
                this._productPricesService.GetBuyerPriceModifiers<PriceModifierDto>(this.User.Identity.Name).FirstOrDefault() ??
                new PriceModifierDto();


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

            foreach (var productListViewModel in viewModel.Products)
            {
                productListViewModel.ModifierValue = priceModifier.PercentValue;
                productListViewModel.PriceModifierType = priceModifier.PriceModifierType;
            }

            return View(viewModel);
        }
    }
}