namespace Vxp.Web.ViewModels.Customers.Products
{
    using System.Collections.Generic;

    public partial class ProductsListViewModel
    {
        public ProductsListViewModel()
        {
            this.Products = new List<ProductListViewModel>();
        }
        public int CategoryFilterId { get; set; }
        public IEnumerable<ProductListViewModel> Products { get; set; }
    }
}