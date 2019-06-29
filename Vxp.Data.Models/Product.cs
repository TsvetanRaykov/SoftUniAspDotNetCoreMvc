﻿using System.Collections.Generic;

namespace Vxp.Data.Models
{
    public class Product : BaseModel
    {
        public Product()
        {
            this.Details = new HashSet<ProductDetail>();
            this.Images = new HashSet<ProductImage>();
            this.Orders = new HashSet<OrderProduct>();
            this.IsAvailable = true;
        }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ProductCategory Category { get; set; }

        public int ProductImageId { get; set; }
        public virtual ProductImage Image { get; set; }

        public virtual ICollection<ProductDetail> Details { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
        public virtual ICollection<OrderProduct> Orders { get; set; }
        public bool IsAvailable { get; set; }
    }
}
