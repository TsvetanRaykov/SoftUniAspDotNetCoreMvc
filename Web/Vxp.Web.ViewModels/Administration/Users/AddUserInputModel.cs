using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Vxp.Services.Mapping;
using Vxp.Services.Models.Administration.Users;
using Vxp.Web.Infrastructure.Attributes.Validation;

namespace Vxp.Web.ViewModels.Administration.Users
{
    public class AddUserInputModel : IMapTo<CreateUserServiceModel>
    {
        public AddUserInputModel()
        {
            this.AvailableRoles = new List<SelectListItem>();
            this.AvailableDistributors = new List<SelectListItem>();
            this.AvailableCountries = new string[] { };
        }

        [Required]
        [DisplayName("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("First name")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} symbols", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Last name")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} must be at least {2} symbols")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Country")]
        [StringLength(50)]
        public string Country { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "City")]
        [StringLength(50)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Address")]
        [StringLength(100)]
        public string Address { get; set; }

        [DisplayName("Contact Email")]
        [RequireVendorPartner(nameof(Role), ErrorMessage = "The Contact Email is required")]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [DisplayName("Phone")]
        [Phone]
        public string ContactPhone { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(36)]
        public string Role { get; set; }

        [DisplayName("Company Name")]
        [RequireVendorPartner(nameof(Role), ErrorMessage = "The Company Name is required")]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [DisplayName("VAT Number")]
        [RequireVendorPartner(nameof(Role), ErrorMessage = "The VAT is required")]
        [StringLength(100)]
        public string CompanyVatNumber { get; set; }

        [DisplayName("Bank Account Number")]
        [RequireVendorPartner(nameof(Role), ErrorMessage = "The Bank Account is required")]
        [StringLength(30)]
        public string AccountNumber { get; set; }

        [DisplayName("BIC Code")]
        [RequireVendorPartner(nameof(Role), ErrorMessage = "The BIC code is required")]
        [StringLength(30)]
        public string BicCode { get; set; }

        [DisplayName("SWIFT")]
        [StringLength(30)]
        public string SwiftCode { get; set; }

        [DisplayName("Bank Name")]
        [RequireVendorPartner(nameof(Role), ErrorMessage = "The Bank name is required")]
        [StringLength(100)]
        public string BankName { get; set; }

        [StringLength(36)] // GUID
        public string DistributorId { get; set; }

        public List<SelectListItem> AvailableRoles { get; set; }
        public ICollection<SelectListItem> AvailableDistributors { get; set; }
        public IEnumerable<string> AvailableCountries { get; set; }

    
    }
}