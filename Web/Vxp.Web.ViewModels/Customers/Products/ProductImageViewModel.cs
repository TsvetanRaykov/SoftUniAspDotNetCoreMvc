namespace Vxp.Web.ViewModels.Customers.Products
{
    using Data.Models;
    using Services.Mapping;

    public class ProductImageViewModel : IMapFrom<ProductImage>
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Alt { get; set; }

        public string Title { get; set; }
    }
}