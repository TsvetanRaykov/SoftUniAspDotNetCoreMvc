using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vxp.Data.Models;
using Vxp.Services.Mapping;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public EditUserProfileViewComponentModel()
        {
            this.BankAccounts = new HashSet<EditUserProfileViewComponentBankAccountModel>();
            this.AvailableCountries = new HashSet<string>();
            this.AvailableDistributors = new HashSet<SelectListItem>();
            this.AvailableRoles = new List<SelectListItem>();
            this.ContactAddress = new EditUserProfileViewComponentAddressModel();
            this.Company = new EditUserProfileViewComponentCompanyModel();
        }

        //AspNetUser
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "First name")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} symbols", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Last name")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} symbols", MinimumLength = 3)]
        public string LastName { get; set; }

        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        //Company
        public EditUserProfileViewComponentCompanyModel Company { get; set; }

        //Address
        public EditUserProfileViewComponentAddressModel ContactAddress { get; set; }

        public IEnumerable<EditUserProfileViewComponentBankAccountModel> BankAccounts { get; set; }


        //Other
        public string RoleId { get; set; }
        public string DistributorId { get; set; }
        public List<SelectListItem> AvailableRoles { get; set; }
        public ICollection<SelectListItem> AvailableDistributors { get; set; }
        public IEnumerable<string> AvailableCountries { get; set; }
        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, EditUserProfileViewComponentModel>()
                .ForMember(dest => dest.RoleId, opt =>
                      opt.MapFrom(src => src.Roles.First().RoleId));
        }
    }
}