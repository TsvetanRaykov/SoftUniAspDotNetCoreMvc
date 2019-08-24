namespace Vxp.Web.ViewModels.Distributor.Products
{
    using System.Collections.Generic;
    using Data.Models;
    using Services.Mapping;
    using Vxp.Data.Common.Enums;

    public class ProductViewModel : IMapFrom<Product>
    {
        public ProductViewModel()
        {
            this.Images = new List<ProductImageViewModel>();
            this.Details = new List<ProductDetailViewModel>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public ProductImageViewModel Image { get; set; }

        public List<ProductImageViewModel> Images { get; set; }

        public List<ProductDetailViewModel> Details { get; set; }

        public decimal BasePrice { get; set; }
        public decimal DistributorPrice
        {
            get
            {
                decimal price;
                switch (this.PriceModifierType)
                {
                    case PriceModifierType.Increase:
                        price = this.BasePrice + (this.BasePrice * this.ModifierValue / 100);
                        break;
                    default:
                        price = this.BasePrice - (this.BasePrice * this.ModifierValue / 100);
                        break;
                }
                return price > 0 ? price : 0m;
            }
        }
        public PriceModifierType PriceModifierType { get; set; }
        public decimal ModifierValue { get; set; }

    }
}