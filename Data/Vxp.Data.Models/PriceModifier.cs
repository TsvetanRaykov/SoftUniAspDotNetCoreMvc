namespace Vxp.Data.Models
{
    using Vxp.Data.Common.Enums;
    using Vxp.Data.Common.Models;

    public class PriceModifier : BaseModel<int>
    {
        public ApplicationUser Seller { get; set; }

        public ApplicationUser Buyer { get; set; }

        public PriceModifierRange PriceModifierRange { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public PriceModifierType PriceModifierType { get; set; }

        public decimal PercentValue { get; set; }
    }
}