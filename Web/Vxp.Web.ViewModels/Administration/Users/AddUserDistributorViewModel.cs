namespace Vxp.Web.ViewModels.Administration.Users
{
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;

    public class AddUserDistributorViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public string CompanyName { get; set; }

        public string DisplayName => $"{this.FirstName} {this.LastName} [{this.Email}] {this.CompanyName}";
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, AddUserDistributorViewModel>()
                .ForMember(dest => dest.CompanyName, opt => opt
                    .MapFrom(src => src.Company != null ? src.Company.Name : null));
        }
    }
}