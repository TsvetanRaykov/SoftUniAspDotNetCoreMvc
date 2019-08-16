using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vxp.Services.Data.Products;
using Vxp.Web.ViewModels.Products;

namespace Vxp.Web.Areas.Vendor.Controllers
{
    public class ProductsController : VendorsController
    {
        private readonly IProductsService _productsService;
        private readonly IProductCategoriesService _productCategoriesService;

        public ProductsController(IProductsService productsService, IProductCategoriesService productCategoriesService)
        {
            this._productsService = productsService;
            this._productCategoriesService = productCategoriesService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return this.View();
        }
        public IActionResult Categories()
        {
            var viewModel = new ProductCategoryInputModel
            {
                ExistingCategories = this._productCategoriesService
                    .GetAllCategories<ProductCategoryInputModel>()
                    .ToList()
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(ProductCategoryInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                await this._productCategoriesService.CreateCategoryAsync(inputModel);
            }

            return this.RedirectToAction(nameof(this.Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory([FromForm] int categoryId)
        {
            await this._productCategoriesService.DeleteCategoryAsync(categoryId);
            return this.RedirectToAction(nameof(this.Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(ProductCategoryInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                await this._productCategoriesService.UpdateCategoryAsync(inputModel.Id, inputModel.Name);
            }
            
            return this.RedirectToAction(nameof(this.Categories));
        }




        public IActionResult Details()
        {
            return this.View();
        }

        #region Remote validation

        [AcceptVerbs("Post")]
        public IActionResult ValidateNewProductCategory([FromForm] string name)
        {
            if (this._productCategoriesService.IsCategoryExist(name))
            {
                return this.Json("This product category already exist.");
            }
            return this.Json(true);
        }

        #endregion
    }
}