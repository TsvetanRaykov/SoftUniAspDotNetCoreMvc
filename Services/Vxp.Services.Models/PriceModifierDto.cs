namespace Vxp.Services.Models
{
    using Vxp.Data.Models;
    using Mapping;
    using Data.Common.Enums;

    public class PriceModifierDto : IMapFrom<PriceModifier>
    {
        public string SellerId { get; set; }
        public string BuyerId { get; set; }
        public PriceModifierType PriceModifierType { get; set; }
        public decimal PercentValue { get; set; }
    }
}