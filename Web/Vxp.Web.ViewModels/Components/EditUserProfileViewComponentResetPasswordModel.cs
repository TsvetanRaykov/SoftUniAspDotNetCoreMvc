﻿using System.ComponentModel.DataAnnotations;

namespace Vxp.Web.ViewModels.Components
{
    public class EditUserProfileViewComponentResetPasswordModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(36)]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}