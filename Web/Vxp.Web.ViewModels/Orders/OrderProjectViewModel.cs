using Vxp.Web.ViewModels.Products;

namespace Vxp.Web.ViewModels.Orders
{
    using Data.Models;
    using Services.Mapping;

    public class OrderProjectViewModel : IMapFrom<Project>, IMapTo<Project>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PartnerId { get; set; }
        public ProductSellerViewModel Partner { get; set; }
        public string OwnerId { get; set; }
        public ProductSellerViewModel Owner { get; set; }
    }
}
