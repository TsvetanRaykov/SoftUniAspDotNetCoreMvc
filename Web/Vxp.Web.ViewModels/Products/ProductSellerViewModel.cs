namespace Vxp.Web.ViewModels.Products
{
    using Vxp.Data.Common.Enums;
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;

    public class ProductSellerViewModel : IHaveCustomMappings
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public PriceModifierType PriceModifierType { get; set; }
        public decimal PercentValue { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, ProductSellerViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.Company.Name ?? $"{src.FirstName} {src.LastName}"));

            configuration.CreateMap<PriceModifier, ProductSellerViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.Seller.Company.Name ?? $"{src.Seller.FirstName} {src.Seller.LastName}"));
        }
    }
}