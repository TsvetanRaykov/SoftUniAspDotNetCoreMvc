using System.Collections.Generic;

namespace Vxp.Web.ViewModels.Distributor.Products
{
    public class ProductsListViewModel
    {
        public ProductsListViewModel()
        {
            this.Products = new List<ProductListViewModel>();
        }
        public int CategoryFilterId { get; set; }
        public IEnumerable<ProductListViewModel> Products { get; set; }
    }
}