using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vxp.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data.Models;
    using Services.Mapping;
    using Microsoft.AspNetCore.Mvc;

    public class ProductInputModel : IMapTo<Product>, IMapFrom<Product>
    {
        public ProductInputModel()
        {
            this.AvailableCategories = new List<SelectListItem>();
        }

        [Required(AllowEmptyStrings = false)]
        [Remote(action: "ValidateProductName", controller: "Products", AdditionalFields = "Category.Name", HttpMethod = "Post")]
        public string Name { get; set; }

        public string Description { get; set; }

        public ProductCategoryInputModel Category { get; set; }

        public ProductImageInputModel Image { get; set; }

        public List<ProductImageInputModel> Images { get; set; }

        public List<ProductDetailInputModel> Details { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }
    }
}