namespace Vxp.Web.ViewModels.Customer.Distributors
{
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;
    using System.Linq;

    public class DistributorListViewModel : IHaveCustomMappings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Projects { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, DistributorListViewModel>()
                .ForMember(dest => dest.Name, opt => opt
                    .MapFrom(src => src.Company.Name ?? $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Email, opt => opt
                    .MapFrom(src => src.ContactAddress.Email ?? src.Email))
                .ForMember(dest => dest.Phone, opt => opt
                    .MapFrom(src => src.ContactAddress.Phone))
                .ForMember(dest => dest.Address, opt => opt
                    .MapFrom(src => string.Join(", ",
                        new[]
                            {
                                src.ContactAddress.AddressLocation, src.ContactAddress.City,
                                src.ContactAddress.CountryName
                            }
                            .Where(e => !string.IsNullOrWhiteSpace(e)))))
                .ForMember(dest => dest.Projects, opt => opt
                    .MapFrom(src => src.Projects.Count));
        }
    }
}