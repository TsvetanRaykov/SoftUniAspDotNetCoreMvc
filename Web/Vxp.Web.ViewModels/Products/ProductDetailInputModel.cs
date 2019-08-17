using System.ComponentModel.DataAnnotations;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Products
{
    public class ProductDetailInputModel : IMapTo<ProductDetail>, IMapFrom<ProductDetail>
    {
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }
        [Range(1, int.MaxValue)]
        public int CommonDetailId { get; set; }
    }
}