namespace Vxp.Web.ViewModels.Distributor.Products
{
    using System.Collections.Generic;
    using Data.Models;
    using Services.Mapping;

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

    }
}