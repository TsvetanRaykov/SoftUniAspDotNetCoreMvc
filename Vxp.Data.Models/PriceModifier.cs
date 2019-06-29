
using Vxp.Data.Models.Enums;

namespace Vxp.Data.Models
{
    public class PriceModifier : BaseModel
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