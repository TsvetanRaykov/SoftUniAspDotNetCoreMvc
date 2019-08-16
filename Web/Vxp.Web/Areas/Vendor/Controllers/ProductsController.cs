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
        private readonly IProductDetailsService _productDetailsService;

        public ProductsController(IProductsService productsService,
            IProductCategoriesService productCategoriesService,
            IProductDetailsService productDetailsService)
        {
            this._productsService = productsService;
            this._productCategoriesService = productCategoriesService;
            this._productDetailsService = productDetailsService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCommonProductDetail(ProductCommonDetailInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                await this._productDetailsService.CreateCommonProductDetailAsync(inputModel);
            }

            return this.RedirectToAction(nameof(this.Details));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCommonProductDetails(ProductCommonDetailInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                await this._productDetailsService.UpdateCommonProductDetailAsync(inputModel);
            }

            return this.RedirectToAction(nameof(this.Details));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommonProductDetail([FromForm] int detailId)
        {
            await this._productDetailsService.DeleteCommonProductDetailAsync(detailId);
            return this.RedirectToAction(nameof(this.Details));
        }

        public IActionResult Details()
        {
            var viewModel = new ProductCommonDetailInputModel
            {
                AllCommonProductDetails = this._productDetailsService
                     .GetAllCommonProductDetails<ProductCommonDetailInputModel>()
                     .ToList()
            };

            return this.View(viewModel);
        }

        #region Remote validation

        [AcceptVerbs("Post")]
        public IActionResult ValidateCommonProductDetail([FromForm] string name, [FromForm] string measure)
        {
            if (this._productDetailsService.IsCommonProductDetailExist(name, measure))
            {
                return this.Json("This property with same measure already exist.");
            }
            return this.Json(true);
        }

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