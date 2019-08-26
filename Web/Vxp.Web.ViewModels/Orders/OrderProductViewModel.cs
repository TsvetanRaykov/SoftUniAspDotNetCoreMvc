namespace Vxp.Web.ViewModels.Orders
{
    using Data.Models;
    using Services.Mapping;
    using Products;

    public class OrderProductViewModel : IMapFrom<Product>
    {
        public int Id { get; set; }
        public ProductImageViewModel Image { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string TotalPrice => $"{(this.Price * this.Quantity):N2}";
    }
}