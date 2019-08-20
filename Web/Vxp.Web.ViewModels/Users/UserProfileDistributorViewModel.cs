namespace Vxp.Web.ViewModels.Users
{
    using System.Linq;
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;

    public class UserProfileDistributorViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public UserProfileAddressInputModel ContactAddress { get; set; }
        public UserProfileCompanyInputModel Company { get; set; }
        public UserProfileBankAccountInputModel BankAccount { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, UserProfileDistributorViewModel>()
                .ForMember(dest => dest.BankAccount, opt => opt
                    .MapFrom(src => src.BankAccounts.FirstOrDefault()));
        }
    }
}
