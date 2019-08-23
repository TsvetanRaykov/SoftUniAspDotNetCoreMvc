using System.Collections.Generic;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Customers.Products
{
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