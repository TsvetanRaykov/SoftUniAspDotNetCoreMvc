namespace Vxp.Web.ViewModels.Customer.Distributors
{
    using AutoMapper;
    using Data.Models;
    using Prices;
    using Services.Mapping;
    using System.Collections.Generic;
    using System.Linq;

    public class DistributorViewModelDetails : IHaveCustomMappings
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PersonalAddress { get; set; }
        public string PersonalPhone { get; set; }
        public string PersonalEmail { get; set; }
        public string BusinessCompanyName { get; set; }
        public string BusinessBin { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessEmail { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string BankBic { get; set; }
        public string BankSwift { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddressPhone { get; set; }
        public string ShippingAddressEmail { get; set; }
        public List<PriceModifierInputModel> PriceModifiers { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, DistributorViewModelDetails>()
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankAccounts.FirstOrDefault().BankName))
                .ForMember(dest => dest.BankAccount,
                    opt => opt.MapFrom(src => src.BankAccounts.FirstOrDefault().AccountNumber))
                .ForMember(dest => dest.BankBic, opt => opt.MapFrom(src => src.BankAccounts.FirstOrDefault().BicCode))
                .ForMember(dest => dest.BankSwift,
                    opt => opt.MapFrom(src => src.BankAccounts.FirstOrDefault().SwiftCode))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.PersonalAddress, opt => opt.MapFrom(src =>
                    string.Join(", ",
                        new[] { src.ContactAddress.AddressLocation, src.ContactAddress.City, src.ContactAddress.CountryName }.Where(e => !string.IsNullOrWhiteSpace(e)))))
                .ForMember(dest => dest.PersonalPhone, opt => opt.MapFrom(src => src.ContactAddress.Phone))
                .ForMember(dest => dest.PersonalEmail, opt => opt.MapFrom(src => src.ContactAddress.Email ?? src.Email))
                .ForMember(dest => dest.BusinessCompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.BusinessBin, opt => opt.MapFrom(src => src.Company.BusinessNumber))
                .ForMember(dest => dest.BusinessAddress, opt => opt.MapFrom(src =>
                    string.Join(", ",
                        new[] { src.Company.ContactAddress.AddressLocation, src.Company.ContactAddress.City, src.Company.ContactAddress.CountryName }.Where(e => !string.IsNullOrWhiteSpace(e)))))
                .ForMember(dest => dest.BusinessPhone, opt => opt.MapFrom(src => src.Company.ContactAddress.Phone))
                .ForMember(dest => dest.BusinessEmail, opt => opt.MapFrom(src => src.Company.ContactAddress.Email))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src =>
                    src.Company.ShippingAddress != null ?
                    string.Join(", ", new[] { src.Company.ShippingAddress.AddressLocation, src.Company.ShippingAddress.City, src.Company.ShippingAddress.CountryName }.Where(e => !string.IsNullOrWhiteSpace(e))) :
                    string.Join(", ", new[] { src.ContactAddress.AddressLocation, src.ContactAddress.City, src.ContactAddress.CountryName }.Where(e => !string.IsNullOrWhiteSpace(e)))))
                .ForMember(dest => dest.ShippingAddressPhone, opt => opt.MapFrom(src => src.Company.ShippingAddress.Phone ?? src.ContactAddress.Phone))
                .ForMember(dest => dest.ShippingAddressEmail, opt => opt.MapFrom(src => src.Company.ShippingAddress.Email ?? src.ContactAddress.Email ?? src.UserName))
                .ForMember(dest => dest.PriceModifiers, opt => opt.MapFrom(src => src.PriceModifiersReceive));
        }
    }
}