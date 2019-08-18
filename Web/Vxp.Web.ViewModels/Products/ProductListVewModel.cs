using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Products
{
    public class ProductListVewModel : IMapFrom<Product>
    {
        public ProductImageInputModel Image { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}