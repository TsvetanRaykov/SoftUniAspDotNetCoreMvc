namespace Vxp.Web.ViewModels.Customer.Products
{
    using Services.Mapping;
    using Services.Models;

    public class ProductPriceViewModel : IMapFrom<PriceModifierDto>
    {
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public decimal Price { get; set; }
        public string PriceFormatted => $"{this.Price:N2}";
    }
}