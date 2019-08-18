﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vxp.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Data.Models;
    using Services.Mapping;

    public class ProductCommonDetailInputModel : IMapTo<CommonProductDetail>, IMapFrom<CommonProductDetail>, IHaveCustomMappings
    {
        public ProductCommonDetailInputModel()
        {
            this.AllCommonProductDetails = new HashSet<ProductCommonDetailInputModel>();
        }
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(30)]
        [Remote(action: "ValidateCommonProductDetail", controller: "Products", AdditionalFields = nameof(Measure) + ",__RequestVerificationToken", HttpMethod = "Post")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(15)]
        public string Measure { get; set; }

        public ICollection<ProductCommonDetailInputModel> AllCommonProductDetails { get; set; }

        public int UsedCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<CommonProductDetail, ProductCommonDetailInputModel>()
                .ForMember(dest => dest.UsedCount, opt => opt
                .MapFrom(src => src.ProductDetails.Count));

            configuration.CreateMap<CommonProductDetail, SelectListItem>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => $"{src.Name}:{src.Measure}"))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id));
        }
    }
}
