using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Customers.Products
{
    public class ProductCommonDetailViewModel : IMapFrom<CommonProductDetail>
    {
        public string Name { get; set; }
        public string Measure { get; set; }
    }
}