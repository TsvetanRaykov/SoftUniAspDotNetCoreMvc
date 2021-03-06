﻿namespace Vxp.Web.ViewModels.Distributor.Products
{
    using Data.Models;
    using Services.Mapping;

    public class ProductCommonDetailViewModel : IMapFrom<CommonProductDetail>
    {
        public string Name { get; set; }
        public string Measure { get; set; }
    }
}