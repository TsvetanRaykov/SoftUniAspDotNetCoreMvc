using AutoMapper;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Vendor.Distributors
{
    public class DistributorListViewModel : IHaveCustomMappings
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Company { get; set; }
        public string ContactEmail { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Discount { get; set; }
        public string DiscountFormatted => $"{this.Discount:N2} %";
        public int ActiveProjects { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, DistributorListViewModel>()
                .ForMember(dest => dest.Company, opt => opt
                    .MapFrom(src =>
                        $"{src.Company.Name}, {src.Company.ContactAddress.City}, {src.Company.ContactAddress.CountryName}"))
                .ForMember(dest => dest.ContactEmail, opt => opt
                    .MapFrom(src => src.Company.ContactAddress.Email))
                .ForMember(dest => dest.ShippingAddress, opt => opt
                    .MapFrom(src =>
                        $"{src.Company.ShippingAddress.AddressLocation}, {src.Company.ShippingAddress.City}, {src.Company.ShippingAddress.CountryName}"));
        }
    }
}