using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vxp.Web.Areas.Administration.ViewModels.Dashboard
{
    public class AddUserInputModel
    {
        public AddUserInputModel()
        {
            this.Roles = new Dictionary<string, string>();
            this.Distributors = new Dictionary<string, string>();
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

        [Required]
        [DisplayName("Contact Email")]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [DisplayName("Phone")]
        [Phone]
        public string ContactPhone { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Role { get; set; }

        [StringLength(100)]
        public string DistributorId { get; set; }

        public Dictionary<string, string> Roles { get; set; }

        public Dictionary<string, string> Distributors { get; set; }
    }
}