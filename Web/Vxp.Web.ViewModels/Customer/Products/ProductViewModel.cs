namespace Vxp.Web.ViewModels.Customer.Products
{
    using System.Collections.Generic;
    using Data.Models;
    using Services.Mapping;
    using Vxp.Web.ViewModels.Products;

    public class ProductViewModel : IMapFrom<Product>
    {
        public ProductViewModel()
        {
            this.Images = new List<ProductImageViewModel>();
            this.Details = new List<ProductDetailViewModel>();
            this.Prices = new List<ProductPriceViewModel>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public decimal BasePrice { get; set; }

        public ProductImageViewModel Image { get; set; }

        public List<ProductImageViewModel> Images { get; set; }

        public List<ProductDetailViewModel> Details { get; set; }

        public List<ProductPriceViewModel> Prices { get; set; }
    }
}