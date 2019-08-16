﻿namespace Vxp.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Vxp.Data.Models;
    using Vxp.Services.Mapping;

    public class ProductCommonDetailInputModel : IMapTo<CommonProductDetail>, IMapFrom<CommonProductDetail>, IHaveCustomMappings
    {
        public ProductCommonDetailInputModel()
        {
            this.AllCommonProductDetails = new HashSet<ProductCommonDetailInputModel>();
        }
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(30)]
        [Remote(action: "ValidateCommonProductDetail", controller: "Products", AdditionalFields = nameof(Measure), HttpMethod = "Post")]
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
        }
    }
}
