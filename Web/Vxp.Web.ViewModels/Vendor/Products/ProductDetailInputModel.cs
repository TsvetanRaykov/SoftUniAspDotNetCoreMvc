using System.ComponentModel.DataAnnotations;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Vendor.Products
{
    public class ProductDetailInputModel : IMapTo<ProductDetail>, IMapFrom<ProductDetail>
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }
        [Range(1, int.MaxValue)]
        public int CommonDetailId { get; set; }

        public ProductCommonDetailInputModel CommonDetail { get; set; }
    }
}