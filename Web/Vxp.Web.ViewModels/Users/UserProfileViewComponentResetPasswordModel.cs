namespace Vxp.Web.ViewModels.Users
{
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Attributes.Validation;

    public class UserProfileViewComponentResetPasswordModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(36)]
        public string ModelUserId { get; set; }

        [RequiredIfNew("")]
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