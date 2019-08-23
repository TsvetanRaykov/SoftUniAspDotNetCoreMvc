using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Customers.Products
{
    public class ProductDetailViewModel : IMapFrom<ProductDetail>
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public ProductCommonDetailViewModel CommonDetail { get; set; }
    }
}