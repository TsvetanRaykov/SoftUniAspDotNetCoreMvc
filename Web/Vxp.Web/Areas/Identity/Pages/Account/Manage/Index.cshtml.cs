namespace Vxp.Web.Areas.Identity.Pages.Account.Manage
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Data.Models;
    using System.Linq;
    using Common;
    using Vxp.Services.Data.Users;
    using Vxp.Web.ViewModels.Users;

    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly IUsersService _usersService;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IUsersService usersService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this._usersService = usersService;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public UserProfileViewModel Input { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            this.Username = user.UserName;

            this.Input = this._usersService
                .GetAll<UserProfileViewModel>(u => u.UserName == this.Username).GetAwaiter().GetResult().FirstOrDefault();

            if (this.Input == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }
            await this._usersService.PopulateCommonUserModelProperties(this.Input);

            if (this.TempData.ContainsKey("UserProfileViewMessage"))
            {
                this.StatusMessage = this.TempData["UserProfileViewMessage"] as string;
            }

            this.Input.StatusMessage = this.StatusMessage;

            //this.IsEmailConfirmed = await this.userManager.IsEmailConfirmedAsync(user);

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            await this._usersService.PopulateCommonUserModelProperties(this.Input);

            if (string.IsNullOrEmpty(this.Input.ContactAddress?.CountryName))
            {
                this.ModelState.AddModelError("ContactAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "CountryName"));
            }

            if (this.Input.RoleName == GlobalConstants.Roles.DistributorRoleName || this.Input.RoleName == GlobalConstants.Roles.VendorRoleName)
            {
                if (!this.Input.BankAccounts.Any())
                {
                    var errorMessage = string.Format(GlobalConstants.ErrorMessages.RequiredField, "Bank account");
                    this.TempData["ErrorMessage"] = errorMessage;
                    this.ModelState.AddModelError("BankAccount", errorMessage);
                }
            }

            if (this.ModelState.IsValid && await this._usersService.UpdateUser(this.Input, new[] { this.Input.RoleName }))
            {
                this.TempData["UserProfileViewMessage"] = $"{this.Input.UserName} data has been updated.";
            }

            await this.signInManager.RefreshSignInAsync(user);

            if (this.TempData.ContainsKey("UserProfileViewMessage"))
            {
                this.StatusMessage = this.TempData["UserProfileViewMessage"] as string;
            }
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            var userId = await this.userManager.GetUserIdAsync(user);
            var email = await this.userManager.GetEmailAsync(user);
            var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = this.Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: this.Request.Scheme);
            await this.emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            this.StatusMessage = "Verification email sent. Please check your email.";
            return this.RedirectToPage();
        }
    }
}
