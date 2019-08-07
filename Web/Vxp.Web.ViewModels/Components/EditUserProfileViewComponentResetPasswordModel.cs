using System.ComponentModel.DataAnnotations;
using Vxp.Web.Infrastructure.Attributes.Validation;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentResetPasswordModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(36)]
        public string ModelUserId { get; set; }

        [RequiredWhenNewUser("")]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string ModelPassword { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare(nameof(ModelPassword), ErrorMessage = "The password and confirmation password do not match.")]
        public string ModelConfirmPassword { get; set; }

    }
}