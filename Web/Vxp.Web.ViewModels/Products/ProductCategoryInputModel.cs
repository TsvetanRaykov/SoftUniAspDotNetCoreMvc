using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vxp.Common;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Products
{
    public class ProductCategoryInputModel : IMapFrom<ProductCategory>, IMapTo<ProductCategory>, IHaveCustomMappings
    {
        public ProductCategoryInputModel()
        {
            this.ExistingCategories = new List<ProductCategoryInputModel>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = GlobalConstants.ErrorMessages.RequiredField)]
        [StringLength(30)]
        [Remote(action: "ValidateNewProductCategory", controller: "Products", HttpMethod = "Post")]
        public string Name { get; set; }

        public int ProductsCount { get; set; }

        [NotMapped]
        public List<ProductCategoryInputModel> ExistingCategories { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ProductCategory, ProductCategoryInputModel>()
                .ForMember(dest => dest.ProductsCount, opt => opt
                    .MapFrom(src => src.Products.Count));
        }
    }
}