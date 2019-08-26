namespace Vxp.Web.ViewModels.Orders
{
    using Products;
    using System.Collections.Generic;
    using Data.Models;
    using Services.Mapping;

    public class OrderInputModel : IMapFrom<Order>, IMapTo<Order>
    {
        public OrderInputModel()
        {
            this.Products = new List<OrderProductViewModel>();
            this.Sellers = new List<ProductSellerViewModel>();
        }

        public List<ProductSellerViewModel> Sellers { get; set; }

        public List<OrderProductViewModel> Products { get; set; }
    }
}