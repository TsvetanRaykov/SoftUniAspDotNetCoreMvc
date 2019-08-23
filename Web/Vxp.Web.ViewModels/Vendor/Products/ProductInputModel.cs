using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vxp.Data.Models;
using Vxp.Services.Mapping;
using Vxp.Web.ViewModels.ModelBinders;

namespace Vxp.Web.ViewModels.Vendor.Products
{
    public class ProductInputModel : IMapTo<Product>, IMapFrom<Product>
    {
        public ProductInputModel()
        {
            this.AvailableCategories = new List<SelectListItem>();
            this.AvailableDetails = new List<SelectListItem>();
            this.IsAvailable = true;
            this.Images = new List<ProductImageInputModel>();
            this.Details = new List<ProductDetailInputModel>();
        }

        public int? Id { get; set; }

        [Display(Name = "Product name")]
        [Required(AllowEmptyStrings = false)]
        [Remote(action: "ValidateProductName", controller: "Products", AdditionalFields = "CategoryId,Id,__RequestVerificationToken", HttpMethod = "Post")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Category")]
        [Required]
        [Range(0, int.MaxValue)]
        public int CategoryId { get; set; }
        public ProductCategoryInputModel Category { get; set; }

        [Display(Name = "Primary image")]
        public IFormFile UploadImage { get; set; }

        [Display(Name = "Gallery images")]
        public List<IFormFile> UploadImages { get; set; }

        [ModelBinder(typeof(FromJsonModelBinder<ProductImageInputModel>))]
        public ProductImageInputModel Image { get; set; }

        [ModelBinder(typeof(FromJsonModelBinder<List<ProductImageInputModel>>))]
        public List<ProductImageInputModel> Images { get; set; }

        [ModelBinder(typeof(FromJsonModelBinder<List<ProductDetailInputModel>>))]
        public List<ProductDetailInputModel> Details { get; set; }

        [Display(Name = "Available")] public bool IsAvailable { get; set; }

        [Display(Name = "Category")]
        public List<SelectListItem> AvailableCategories { get; set; }

        [Display(Name = "Add property")]
        public List<SelectListItem> AvailableDetails { get; set; }

        [Display(Name = "Base price €")]
        [Required]
        [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "The minimum base price is € {1}")]
        public decimal BasePrice { get; set; }
    }
}