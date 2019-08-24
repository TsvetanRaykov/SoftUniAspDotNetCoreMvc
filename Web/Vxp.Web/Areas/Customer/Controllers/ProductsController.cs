namespace Vxp.Web.Areas.Customer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Services.Data.Products;
    using Vxp.Data.Common.Enums;
    using Services.Models;
    using Vxp.Web.ViewModels.Customer.Products;

    public class ProductsController : CustomersController
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

        public IActionResult Product(int id)
        {
            var product = this._productsService.GetAllProducts<ProductViewModel>().FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var priceModifiers =
                this._productPricesService.GetBuyerPriceModifiers<PriceModifierDto>(this.User.Identity.Name);

            foreach (var priceModifier in priceModifiers)
            {
                var priceModel = AutoMapper.Mapper.Map<ProductPriceViewModel>(priceModifier);

                if (priceModifier.PriceModifierType == PriceModifierType.Decrease)
                {
                    priceModel.Price = product.BasePrice - (product.BasePrice * priceModifier.PercentValue / 100);
                }
                else
                {
                    priceModel.Price = product.BasePrice + (product.BasePrice * priceModifier.PercentValue / 100);
                }
                product.Prices.Add(priceModel);
            }

            product.Images.Insert(0, product.Image);

            return this.View(product);
        }
    }
}