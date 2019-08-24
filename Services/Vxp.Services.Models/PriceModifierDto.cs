namespace Vxp.Services.Models
{
    using AutoMapper;
    using Vxp.Data.Models;
    using Mapping;
    using Data.Common.Enums;

    public class PriceModifierDto : IHaveCustomMappings
    {
        public string SellerName { get; set; }
        public string SellerId { get; set; }
        public string BuyerId { get; set; }
        public PriceModifierType PriceModifierType { get; set; }
        public decimal PercentValue { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<PriceModifier, PriceModifierDto>()
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src =>
                    src.Seller.Company.Name ?? $"{src.Seller.FirstName} {src.Seller.LastName}"));
        }
    }
}