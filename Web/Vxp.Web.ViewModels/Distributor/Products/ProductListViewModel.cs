namespace Vxp.Web.ViewModels.Distributor.Products
{
    using Data.Models;
    using Services.Mapping;

    public class ProductListViewModel : IMapFrom<Product>
    {
        public ProductImageViewModel Image { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
    }
}