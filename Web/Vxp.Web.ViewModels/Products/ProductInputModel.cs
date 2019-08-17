using Vxp.Web.ViewModels.ModelBinders;

namespace Vxp.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data.Models;
    using Services.Mapping;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ProductInputModel : IMapTo<Product>, IMapFrom<Product>
    {
        public ProductInputModel()
        {
            this.AvailableCategories = new List<SelectListItem>();
            this.AvailableDetails = new List<SelectListItem>();
            this.IsAvailable = true;
            this.Images = new List<ProductImageInputModel>();
        }

        [Display(Name = "Product name")]
        [Required(AllowEmptyStrings = false)]
        [Remote(action: "ValidateProductName", controller: "Products", AdditionalFields = "Category.Name", HttpMethod = "Post")]
        public string Name { get; set; }

        public string Description { get; set; }

        public ProductCategoryInputModel Category { get; set; }

        [Display(Name = "Primary image")]
        [Required]
        public IFormFile UploadImage { get; set; }

        [Display(Name = "Gallery images")]
        public List<IFormFile> UploadImages { get; set; }

        public ProductImageInputModel Image { get; set; }

        public List<ProductImageInputModel> Images { get; set; }

        [ModelBinder(typeof(ProductDetailsFromJsonModelBinder))]
        public List<ProductDetailInputModel> Details { get; set; }

        [Display(Name = "Available")] public bool IsAvailable { get; set; }

        [Display(Name = "Category")]
        public List<SelectListItem> AvailableCategories { get; set; }

        [Display(Name = "Add property")]
        public List<SelectListItem> AvailableDetails { get; set; }
    }
}