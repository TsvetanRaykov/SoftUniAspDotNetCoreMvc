namespace Vxp.Web.Areas.Identity.Pages.Account
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Models;
    using Common;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class LoginModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, IEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            this._signInManager = signInManager;
            this._logger = logger;
            this._emailSender = emailSender;
            this._userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.User.Identity.IsAuthenticated)
            {
                if (this.User.IsInRole(GlobalConstants.Roles.AdministratorRoleName))
                {
                    return this.RedirectToAction("Index", "Dashboard", new { area = "Administration" });
                }

                return this.LocalRedirect(returnUrl);
            }

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            this.ReturnUrl = returnUrl;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await this._signInManager.PasswordSignInAsync(this.Input.Email, this.Input.Password, this.Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {

                    this._logger.LogInformation("User logged in.");

                    //return this.LocalRedirect(returnUrl);
                    return this.RedirectToPage("./Login");
                }

                if (result.RequiresTwoFactor)
                {
                    return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = this.Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    this._logger.LogWarning("User account locked out.");
                    return this.RedirectToPage("./Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
    }
}
