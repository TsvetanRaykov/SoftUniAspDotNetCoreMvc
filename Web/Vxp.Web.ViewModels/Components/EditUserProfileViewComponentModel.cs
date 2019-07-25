using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vxp.Data.Models;
using Vxp.Services.Mapping;
using Vxp.Web.Infrastructure.ModelBinders;

namespace Vxp.Web.ViewModels.Components
{
    
    public class EditUserProfileViewComponentModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public EditUserProfileViewComponentModel()
        {
            this.BankAccounts = new HashSet<SelectListItem>();
            this.AvailableCountries = new HashSet<string>();
            this.AvailableDistributors = new HashSet<SelectListItem>();
            this.AvailableRoles = new List<SelectListItem>();
            this.ContactAddress = new EditUserProfileViewComponentAddressModel();
            this.Company = new EditUserProfileViewComponentCompanyModel();
        }

        //AspNetUser
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; }

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
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //Company
        public EditUserProfileViewComponentCompanyModel Company { get; set; }

        //Address
        public EditUserProfileViewComponentAddressModel ContactAddress { get; set; }

        [ModelBinder(typeof(KvpStringListToSelectListModelBinder))]
        public IEnumerable<SelectListItem> BankAccounts { get; set; }

        //Other
        //[Required(AllowEmptyStrings = false)]
        //[Display(Name = "Role")]
        //[StringLength(32)]
        public string RoleId { get; set; }

        [Display(Name = "Distributor")]
        [StringLength(32)]
        public string DistributorId { get; set; }

        public List<SelectListItem> AvailableRoles { get; set; }

        [ModelBinder(typeof(KvpStringListToSelectListModelBinder))]
        public ICollection<SelectListItem> AvailableDistributors { get; set; }

        public IEnumerable<string> AvailableCountries { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationUser, EditUserProfileViewComponentModel>()
                .ForMember(dest => dest.RoleId, opt =>
                    opt.MapFrom(src => src.Roles.First().RoleId))
                .ForMember(dest => dest.AvailableDistributors,
                    opt => opt.MapFrom(src => src.Distributors.Select(d => new SelectListItem
                    {
                        Value = d.DistributorKey.BankAccount.Owner.Id,
                        Text = $"{d.DistributorKey.BankAccount.Owner.Company.Name}"
                    })));
        }
    }
}