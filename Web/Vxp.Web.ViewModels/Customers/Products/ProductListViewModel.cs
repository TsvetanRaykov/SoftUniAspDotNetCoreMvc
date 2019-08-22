using System.Collections.Generic;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Customers.Products
{
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