using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Products
{
    public class ProductDetailInputModel : IMapTo<ProductDetail>, IMapFrom<ProductDetail>
    {
        public string Value { get; set; }

        public ProductInputModel Product { get; set; }

        public ProductCommonDetailInputModel CommonDetail { get; set; }
    }
}