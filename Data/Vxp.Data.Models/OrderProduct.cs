﻿namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Models;

    public class OrderProduct : BaseDeletableModel<int>
    {
        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string ProductData { get; set; }
        public string PriceModifierData { get; set; }
    }
}