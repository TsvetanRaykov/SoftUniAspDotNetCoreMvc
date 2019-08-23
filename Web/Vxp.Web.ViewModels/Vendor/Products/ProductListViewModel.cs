namespace Vxp.Web.ViewModels.Vendor.Products
{
    using Data.Models;
    using Services.Mapping;

    public class ProductListViewModel : IMapFrom<Product>
    {
        public ProductImageInputModel Image { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public int CategoryId { get; set; }
    }
}