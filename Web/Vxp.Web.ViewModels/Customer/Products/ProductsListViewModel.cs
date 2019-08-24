namespace Vxp.Web.ViewModels.Customer.Products
{
    using System.Collections.Generic;

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