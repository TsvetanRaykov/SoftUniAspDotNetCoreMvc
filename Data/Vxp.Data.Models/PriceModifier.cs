﻿namespace Vxp.Data.Models
{
    using Common.Enums;
    using Vxp.Data.Common.Models;

    public class PriceModifier : BaseDeletableModel<int>
    {
        public string SellerId { get; set; }
        public virtual ApplicationUser Seller { get; set; }

        public string BuyerId { get; set; }
        public virtual ApplicationUser Buyer { get; set; }

        public PriceModifierRange PriceModifierRange { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public PriceModifierType PriceModifierType { get; set; }

        public decimal PercentValue { get; set; }
    }
}