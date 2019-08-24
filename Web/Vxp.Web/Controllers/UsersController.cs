namespace Vxp.Web.Controllers
{
    using System.Linq;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Data.Models;
    using Vxp.Services.Data.Users;
    using ViewModels.Users;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersService _usersService;
        private readonly IEmailSender _emailSender;

        public UsersController(IUsersService usersService, IEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            this._usersService = usersService;
            this._emailSender = emailSender;
            this._userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UserProfileInputModel inputModel)
        {
            var validationPropertyNames = new[] { nameof(inputModel.UserId), nameof(inputModel.Password), nameof(inputModel.ConfirmPassword) };

            foreach (var property in this.ModelState)
            {
                if (!validationPropertyNames.Contains(property.Key))
                {
                    this.ModelState[property.Key].Errors.Clear();
                    this.ModelState[property.Key].ValidationState = ModelValidationState.Valid;
                }
            }

            var userModels = await this._usersService
                .GetAll<UserProfileInputModel>(u => u.Id == inputModel.UserId);

            var userModel = userModels.Single();
            await this._usersService.PopulateCommonUserModelPropertiesAsync(userModel);

            if (this.ModelState.IsValid)
            {
                if (await this._usersService.UpdateUserPasswordAsync(inputModel.UserId, inputModel.Password))
                {
                    this.TempData["UserProfileViewMessage"] = $"{userModel.UserName} password has been set.";
                }
            }

            return this.RedirectToAction("EditUser", "Dashboard", new { id = inputModel.UserId, area = "Administration" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmailAsync([FromForm] string userId)
        {
            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
            {
                this.TempData["ErrorMessage"] = $"Unable to load user with ID '{userId}'.";
                return this.Ok();
            }

            var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = this.Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: this.Request.Scheme);


            await this._emailSender.SendEmailAsync(user.UserName,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            this.TempData["UserProfileViewMessage"] = "Verification email sent. Please check your email.";

            return this.Ok();
        }

        #region Remote Validations

        [AcceptVerbs("Post")]
        public IActionResult ValidateBusinessNumber([FromForm(Name = "Company.Name")] string companyName, [FromForm(Name = "Company.BusinessNumber")] string businessNumber)
        {
            if (string.IsNullOrWhiteSpace(companyName) && !string.IsNullOrEmpty(businessNumber))
            {
                return this.Json("The Business number require Company.");
            }

            return this.Json(true);
        }

        [AcceptVerbs("Post")]
        public IActionResult ValidateCompanyName([FromForm(Name = "Company.Name")] string companyName, [FromForm(Name = "Company.BusinessNumber")] string businessNumber)
        {
            if (string.IsNullOrWhiteSpace(businessNumber) && !string.IsNullOrEmpty(companyName))
            {
                return this.Json("The Company must have Business number.");
            }

            return this.Json(true);
        }

        [AcceptVerbs("Post")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateNewUsername(string userName, bool isNewUser)
        {
            if (isNewUser && await this._usersService.IsRegistered(userName))
            {
                return this.Json("This email is already in use.");
            }
            return this.Json(true);
        }

        #endregion
    }
}
