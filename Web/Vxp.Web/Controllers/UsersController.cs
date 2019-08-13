using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Vxp.Data.Models;
using Vxp.Services.Data.Users;
using Vxp.Web.Controllers;
using Vxp.Web.ViewModels.Users;

namespace Vxp.Web.Areas.Users.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {

        private readonly IUsersService _usersService;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(IUsersService usersService, RoleManager<ApplicationRole> roleManager)
        {
            this._usersService = usersService;
            this._roleManager = roleManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UserProfileViewModel inputModel)
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
                .GetAll<UserProfileViewModel>(u => u.Id == inputModel.UserId);

            var userModel = userModels.Single();
            await this._usersService.PopulateCommonUserModelProperties(userModel);

            if (this.ModelState.IsValid)
            {
                if (await this._usersService.UpdateUserPasswordAsync(inputModel.UserId, inputModel.Password))
                {
                    this.TempData["UserProfileViewMessage"] = $"{userModel.UserName} password has been set.";
                }
            }

            return this.RedirectToAction("Update", new { id = inputModel.UserId });
        }

        public IActionResult Profile()
        {
            return this.View();
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
        public async Task<IActionResult> ValidateNewUsername([FromForm] string userName, [FromForm] bool isNewUser)
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
