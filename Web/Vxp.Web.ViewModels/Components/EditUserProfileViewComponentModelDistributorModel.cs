using System.Linq;
using AutoMapper;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentModelDistributorModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public EditUserProfileViewComponentAddressModel ContactAddress { get; set; }
        public EditUserProfileViewComponentCompanyModel Company { get; set; }
        public EditUserProfileViewComponentModelBankAccountModel BankAccount { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, EditUserProfileViewComponentModelDistributorModel>()
                .ForMember(dest => dest.BankAccount, opt => opt
                    .MapFrom(src => src.BankAccounts.FirstOrDefault()));
        }
    }
}