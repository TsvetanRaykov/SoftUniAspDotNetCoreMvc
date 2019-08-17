namespace Vxp.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Common;
    using Data.Models;
    using Services.Mapping;

    public class ProductCategoryInputModel : IMapFrom<ProductCategory>, IMapTo<ProductCategory>, IHaveCustomMappings
    {
        public ProductCategoryInputModel()
        {
            this.ExistingCategories = new List<ProductCategoryInputModel>();
        }

        public int Id { get; set; }

        [Display(Name = "Category")]
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

            configuration.CreateMap<ProductCategory, SelectListItem>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Name));
        }
    }
}