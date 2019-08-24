namespace Vxp.Web.ViewModels.Distributor.Products
{
    using Data.Models;
    using Services.Mapping;

    public class ProductDetailViewModel : IMapFrom<ProductDetail>
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public ProductCommonDetailViewModel CommonDetail { get; set; }
    }
}