namespace Vxp.Web.ViewModels.Customer.Products
{
    using Vxp.Web.ViewModels.Products;
    using System.Collections.Generic;
    using Data.Models;
    using Services.Mapping;

    public class ProductListViewModel : IMapFrom<Product>
    {
        public ProductImageViewModel Image { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        public List<decimal> Prices { get; set; }
    }
}