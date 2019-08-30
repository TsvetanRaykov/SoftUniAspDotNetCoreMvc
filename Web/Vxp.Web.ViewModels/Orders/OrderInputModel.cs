namespace Vxp.Web.ViewModels.Orders
{
    using System;
    using Vxp.Data.Common.Enums;
    using System.Linq;
    using Data.Models;
    using Products;
    using Services.Mapping;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OrderInputModel : IMapFrom<Order>, IMapTo<Order>
    {
        public OrderInputModel()
        {
            this.Products = new List<OrderProductViewModel>();
            this.AvailableProjects = new List<OrderProjectViewModel>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(36, MinimumLength = 36)]
        public string SellerId { get; set; }

        public ProductSellerViewModel Seller { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(36, MinimumLength = 36)]
        public string BuyerId { get; set; }

        public ProductSellerViewModel Buyer { get; set; }

        public List<OrderProductViewModel> Products { get; set; }

        [Required]
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