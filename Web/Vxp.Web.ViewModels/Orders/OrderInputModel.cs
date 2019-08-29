namespace Vxp.Web.ViewModels.Orders
{
    using System;
    using Vxp.Data.Common.Enums;
    using System.Linq;
    using Data.Models;
    using Products;
    using Services.Mapping;
    using System.Collections.Generic;

    public class OrderInputModel : IMapFrom<Order>, IMapTo<Order>
    {
        public OrderInputModel()
        {
            this.Products = new List<OrderProductViewModel>();
          //  this.Sellers = new List<ProductSellerViewModel>();
            this.AvailableProjects = new List<OrderProjectViewModel>();
        }

        public string SellerId { get; set; }

        public ProductSellerViewModel Seller { get; set; }

        //public List<ProductSellerViewModel> Sellers { get; set; }

        public List<OrderProductViewModel> Products { get; set; }

        public int ProjectId { get; set; }

        public List<OrderProjectViewModel> AvailableProjects { get; set; }


        public string TotalPrice
        {
            get { return $"{this.Products.Sum(p => p.Price * p.Quantity):N2}"; }
        }

        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public OrderStatus Status { get; set; }

        public bool Create { get; set; }
    }
}