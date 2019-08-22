namespace Vxp.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using Data.Models;
    using Common;
    using Vxp.Services.Data.Users;
    using Services.Mapping;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class RegisterModel : PageModel, IMapTo<ApplicationUser>
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IUsersService usersService;
        private readonly IDistributorsService distributorsService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUsersService usersService,
            IDistributorsService distributorsService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.usersService = usersService;
            this.distributorsService = distributorsService;
        }

        public string ReturnUrl { get; set; }

        public IActionResult OnGet(string returnUrl = null)
        {
            if (this.signInManager.IsSignedIn(this.User))
            {
                return this.LocalRedirect("~/");
            }

            this.IsNewUser = true;

            if (this.HttpContext.Request.Query.ContainsKey("key"))
            {
                this.DistributorKey = this.HttpContext.Request.Query["key"];
            }
            this.ReturnUrl = returnUrl;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (!this.ModelState.IsValid) return this.Page();

            var newUserId = await this.usersService.CreateUserAsync(this, this.InputPasswords.Password, GlobalConstants.Roles.CustomerRoleName);

            if (!string.IsNullOrEmpty(newUserId))
            {
                this.logger.LogInformation("User created a new account with password.");

                var user = await this.userManager.FindByIdAsync(newUserId);

                await this.distributorsService.AddCustomerToDistributorAsync(user.UserName, this.DistributorKey);

                var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = this.Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = user.Id, code = code },
                    protocol: this.Request.Scheme);

                await this.emailSender.SendEmailAsync(
                    this.UserName,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                await this.signInManager.SignInAsync(user, isPersistent: false);
                return this.LocalRedirect(returnUrl);
            }

            return this.Page();
        }

        [BindProperty]
        [Required]
        public bool IsNewUser { get; set; }

        [BindProperty]
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [Remote(action: "ValidateNewUsername", controller: "Users", AdditionalFields = nameof(IsNewUser) + ",__RequestVerificationToken", HttpMethod = "Post")]
        public string UserName { get; set; }

        [BindProperty]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [BindProperty]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [BindProperty]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Distributor Key")]
        public string DistributorKey { get; set; }

        [BindProperty]
        public Passwords InputPasswords { get; set; }

        public class Passwords
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
    }
}
