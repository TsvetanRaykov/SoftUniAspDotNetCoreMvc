using System.ComponentModel.DataAnnotations;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Products
{
    public class ProductImageInputModel : IMapTo<ProductImage>, IMapFrom<ProductImage>
    {
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.ImageUrl)]
        public string Url { get; set; }

        [StringLength(50)]
        public string Alt { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public ProductInputModel Product { get; set; }
    }
}