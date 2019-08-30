namespace Vxp.Web.ViewModels.Distributor.Customers
{
    using System.Collections.Generic;
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;
    using Prices;
    using System.Linq;

    public class CustomersListViewModel : IHaveCustomMappings
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ShippingAddress { get; set; }
        public int Projects { get; set; }
        public List<PriceModifierInputModel> PriceModifiers { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, CustomersListViewModel>()
                .ForMember(dest => dest.Name, opt => opt
                    .MapFrom(src => src.Company.Name ?? $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.ContactEmail, opt => opt
                    .MapFrom(src => src.ContactAddress.Email ?? src.Email))
                .ForMember(dest => dest.ShippingAddress, opt => opt
                    .MapFrom(src => 
                        src.Company.ShippingAddress != null ?
                        string.Join(", ",
                        new[] { src.Company.ShippingAddress.AddressLocation, src.Company.ShippingAddress.City, src.Company.ShippingAddress.CountryName }
                            .Where(e => !string.IsNullOrWhiteSpace(e))) :
                        string.Join(", ",
                            new[] { src.ContactAddress.AddressLocation, src.ContactAddress.City, src.ContactAddress.CountryName }
                                .Where(e => !string.IsNullOrWhiteSpace(e)))
                        ))
                .ForMember(dest => dest.Projects, opt => opt
                    .MapFrom(src => src.Projects.Count))
                .ForMember(dest => dest.PriceModifiers, opt => opt
                    .MapFrom(src => src.PriceModifiersReceive));
        }
    }
}