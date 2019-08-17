using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vxp.Services;
using Vxp.Services.Data.Products;
using Vxp.Web.ViewModels.Products;

namespace Vxp.Web.Areas.Vendor.Controllers
{
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
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductInputModel
            {
                AvailableCategories = await this._productCategoriesService.GetAllCategories<SelectListItem>().ToListAsync(),
                AvailableDetails = await this._productDetailsService.GetAllCommonProductDetails<SelectListItem>().ToListAsync()
            };

            viewModel.AvailableCategories.Add(new SelectListItem("- Select Category -", null, true, true));
            viewModel.AvailableDetails.Add(new SelectListItem("- Select Property -", null, true, true));

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInputModel inputModel)
        {

            if (this.ModelState.IsValid)
            {
                string imageUrl = await this._cloudinaryService.UploadImage(inputModel.UploadImage, inputModel.Name);
                inputModel.Image = new ProductImageInputModel
                {
                    Alt = inputModel.Name,
                    Title = inputModel.Name,
                    Url = HttpUtility.UrlEncode(imageUrl)
                };

                if (inputModel.UploadImages != null)
                {
                    for (var index = 0; index < inputModel.UploadImages.Count; index++)
                    {
                        var formFile = inputModel.UploadImages[index];
                        var imageName = $"{inputModel.Name}_view_{index:D2}";
                        imageUrl = await this._cloudinaryService.UploadImage(formFile, imageName);
                        inputModel.Images.Add(new ProductImageInputModel
                        {
                            Alt = imageName,
                            Title = imageName,
                            Url = HttpUtility.UrlEncode(imageUrl)
                        });
                    }
                }

                await this._productsService.CreateProductAsync(inputModel);
            }

            return this.View(inputModel);
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

        [AcceptVerbs("Post")]
        public IActionResult ValidateProductName([FromForm] string name, [FromForm(Name = "Category.Name")] string categoryName)
        {
            if (this._productsService.IsProductExist(name, categoryName))
            {
                return this.Json("The product already exist in that category.");
            }
            return this.Json(true);
        }

        #endregion
    }
}