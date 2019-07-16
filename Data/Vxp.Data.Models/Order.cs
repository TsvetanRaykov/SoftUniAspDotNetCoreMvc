// ReSharper disable VirtualMemberCallInConstructor
namespace Vxp.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Vxp.Data.Common.Enums;
    using Vxp.Data.Common.Models;

    public class Order : BaseDeletableModel<int>
    {
        public Order()
        {
            this.Products = new HashSet<OrderProduct>();
            this.Status = OrderStatus.Draft;
        }

        public OrderStatus Status { get; set; }

        public DateTime Deadline { get; set; }

        public string Note { get; set; }

        public string SellerId { get; set; }

        public virtual ApplicationUser Seller { get; set; }

        public virtual ApplicationUser Buyer { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<OrderProduct> Products { get; set; }

    }
}