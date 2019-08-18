using System;
using Microsoft.AspNetCore.Http;

namespace Vxp.Web.Areas.Vendor.Controllers
{
    using System.Web;
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Services;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels.Products;
    using Vxp.Services.Data.Products;

    public class ProductsController : VendorsController
    {
        private readonly IProductsService _productsService;
        private readonly IProductCategoriesService _productCategoriesService;
        private readonly IProductDetailsService _productDetailsService;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(IProductsService productsService,
            IProductCategoriesService productCategoriesService,
            IProductDetailsService productDetailsService, ICloudinaryService cloudinaryService)
        {
            this._productsService = productsService;
            this._productCategoriesService = productCategoriesService;
            this._productDetailsService = productDetailsService;
            this._cloudinaryService = cloudinaryService;
        }

        public IActionResult Index()
        {
            var products = this._productsService.GetAllProducts<ProductListVewModel>();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductInputModel();
            await PopulateCommonProductViewModelProperties(viewModel);
            viewModel.AvailableCategories.Add(new SelectListItem("- Select Category -", null, true, true));
            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInputModel inputModel)
        {
            if (inputModel.UploadImage == null)
            {
                this.ModelState.AddModelError("UploadImage", string.Format(GlobalConstants.ErrorMessages.RequiredField, "Primary image"));
            }

            await PopulateCommonProductViewModelProperties(inputModel);

            if (!this.ModelState.IsValid) return this.View(inputModel);

            await PopulateProductViewImages(inputModel);

            var newProduct = await this._productsService.CreateProductAsync(inputModel);

            return this.RedirectToAction("Edit", new { id = newProduct.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = this._productsService.GetAllProducts<ProductInputModel>().FirstOrDefault(p => p.Id == id);
            if (viewModel == null) { return this.NotFound(); }
            await PopulateCommonProductViewModelProperties(viewModel);
            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductInputModel inputModel)
        {
            await PopulateCommonProductViewModelProperties(inputModel);

            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await PopulateProductViewImages(inputModel);
            await this._productsService.UpdateProductAsync(inputModel);

            return this.RedirectToAction(nameof(Edit), new { id = inputModel.Id });
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
        [ValidateAntiForgeryToken]
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

        [AcceptVerbs("Post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateProductName(ProductInputModel viewModel)
        {
            if (await this._productsService.IsProductExist(viewModel))
            {
                return this.Json("The product already exist in that category.");
            }
            return this.Json(true);
        }

        #endregion

        private async Task PopulateCommonProductViewModelProperties(ProductInputModel viewModel)
        {
            viewModel.AvailableCategories = await this._productCategoriesService.GetAllCategories<SelectListItem>().ToListAsync();
            viewModel.AvailableCategories.ForEach(c => c.Selected = c.Value == viewModel.Category?.Id.ToString());
            viewModel.AvailableDetails = await this._productDetailsService.GetAllCommonProductDetails<SelectListItem>().ToListAsync();
            viewModel.AvailableDetails.Add(new SelectListItem("- Select Property -", null, true, true));

            var primaryImage = viewModel.Images.FirstOrDefault(i => i.Id == viewModel.Image.Id);
            if (primaryImage != null) { viewModel.Images.Remove(primaryImage); }
        }

        private async Task PopulateProductViewImages(ProductInputModel viewModel)
        {
            if (viewModel.UploadImage != null)
            {
                var imageUrl = await this._cloudinaryService.UploadImage(viewModel.UploadImage, viewModel.Name);
                viewModel.Image = new ProductImageInputModel
                {
                    Alt = viewModel.Name,
                    Title = viewModel.Name,
                    Url = HttpUtility.UrlEncode(imageUrl)
                };
            }

            if (viewModel.UploadImages != null)
            {
                foreach (var formFile in viewModel.UploadImages)
                {
                    var imageName = $"{viewModel.Name}_view_{new Random().Next(100):D3}";
                    var imageUrl = await this._cloudinaryService.UploadImage(formFile, imageName);
                    viewModel.Images.Add(new ProductImageInputModel
                    {
                        Alt = imageName,
                        Title = viewModel.Name,
                        Url = HttpUtility.UrlEncode(imageUrl)
                    });
                }
            }
        }
    }
}