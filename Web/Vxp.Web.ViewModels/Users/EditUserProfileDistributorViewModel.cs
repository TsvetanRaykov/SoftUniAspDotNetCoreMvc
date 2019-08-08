namespace Vxp.Web.ViewModels.Users
{
    using System.Linq;
    using AutoMapper;
    using Data.Models;
    using Services.Mapping;

    public class EditUserProfileDistributorViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public EditUserProfileAddressViewModel ContactAddress { get; set; }
        public EditUserProfileCompanyViewModel Company { get; set; }
        public EditUserProfileBankAccountViewModel BankAccount { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, EditUserProfileDistributorViewModel>()
                .ForMember(dest => dest.BankAccount, opt => opt
                    .MapFrom(src => src.BankAccounts.FirstOrDefault()));
        }
    }
}
