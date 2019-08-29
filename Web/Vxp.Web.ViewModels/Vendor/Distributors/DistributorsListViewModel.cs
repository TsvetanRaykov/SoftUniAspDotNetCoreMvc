namespace Vxp.Web.ViewModels.Vendor.Distributors
{
    using AutoMapper;
    using Data.Models;
    using Prices;
    using Services.Mapping;
    using System.Collections.Generic;
    using System.Linq;

    public class DistributorsListViewModel : IHaveCustomMappings
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Company { get; set; }
        public string ContactEmail { get; set; }
        public string ShippingAddress { get; set; }
        public int ActiveProjects { get; set; }
        public List<PriceModifierInputModel> PriceModifiers { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, DistributorsListViewModel>()
                .ForMember(dest => dest.Company, opt => opt
                    .MapFrom(src => string.Join(", ",
                        new[] { src.Company.Name, src.Company.ContactAddress.City, src.Company.ContactAddress.CountryName }
                            .Where(e => !string.IsNullOrWhiteSpace(e)))))
                .ForMember(dest => dest.ContactEmail, opt => opt
                    .MapFrom(src => src.Company.ContactAddress.Email ?? src.Email))
                .ForMember(dest => dest.ShippingAddress, opt => opt
                    .MapFrom(src => string.Join(", ",
                        new[] { src.Company.ShippingAddress.AddressLocation, src.Company.ShippingAddress.City, src.Company.ShippingAddress.CountryName }
                            .Where(e => !string.IsNullOrWhiteSpace(e)))))
                .ForMember(dest => dest.PriceModifiers, opt => opt
                    .MapFrom(src => src.PriceModifiersReceive));
        }
    }
}