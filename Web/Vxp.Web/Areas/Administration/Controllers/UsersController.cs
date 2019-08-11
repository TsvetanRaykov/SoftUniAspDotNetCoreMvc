namespace Vxp.Web.Areas.Administration.Controllers
{
    using Common;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Services.Mapping;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using ViewModels.Users;
    using Vxp.Services.Data.Users;
    using Vxp.Web.ViewModels.Administration.Users;

    public class UsersController : AdministrationController
    {

        private readonly IUsersService _usersService;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(IUsersService usersService, RoleManager<ApplicationRole> roleManager)
        {
            this._usersService = usersService;
            this._roleManager = roleManager;
        }

        public async Task<IActionResult> List()
        {
            var viewModel = await this._usersService.GetAll<ListUserViewModel>().GetAwaiter().GetResult().ToListAsync();
            return this.View("ListUsers", viewModel);
        }

        public async Task<IActionResult> Create()
        {

            var viewModel = new UserProfileViewModel
            {
                RoleName = GlobalConstants.Roles.AdministratorRoleName, // Role dropdown selected item by default
                IsNewUser = true,
            };

            await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(viewModel);

            return this.View("CreateUser", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfileViewModel inputModel)
        {

            if (string.IsNullOrEmpty(inputModel.ContactAddress?.CountryName))
            {
                this.ModelState.AddModelError("ContactAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "CountryName"));
            }

            if (!this.ModelState.IsValid)
            {
                await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(inputModel);


                if (this.ModelState[nameof(inputModel.Password)].ValidationState == ModelValidationState.Invalid)
                {
                    TempData["ErrorMessage"] = this.ModelState[nameof(inputModel.Password)].Errors.FirstOrDefault()?.ErrorMessage;
                }

                return this.View("CreateUser", inputModel);
            }

            var newUserId = await this._usersService.CreateUser(inputModel, inputModel.Password, inputModel.RoleName);

            if (!string.IsNullOrEmpty(newUserId))
            {
                this.TempData["UserProfileViewMessage"] = $"{inputModel.UserName} has been created.";
            }
            else
            {
                TempData["ErrorMessage"] = $"{inputModel.UserName} cannot be created.";
            }
            return this.RedirectToAction(nameof(this.Update), new { id = newUserId });
        }

        public async Task<IActionResult> Update(UserIdInputModel inputModel)
        {

            var userModels = await this._usersService
                .GetAll<UserProfileViewModel>(u => u.Id == inputModel.Id);

            var userModel = userModels.FirstOrDefault();

            userModel = userModel ?? new UserProfileViewModel
            {
                IsNewUser = true
            };

            await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(userModel);

            if (this.TempData.ContainsKey("UserProfileViewMessage"))
            {
                userModel.SuccessMessage = this.TempData["UserProfileViewMessage"] as string;
            }

            return this.View("EditUser", userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserProfileViewModel inputModel)
        {
            await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(inputModel);

            if (string.IsNullOrEmpty(inputModel.ContactAddress?.CountryName))
            {
                this.ModelState.AddModelError("ContactAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "CountryName"));
            }

            if (this.ModelState.IsValid && await this._usersService.UpdateUser(inputModel, new[] { inputModel.RoleName }))
            {
                this.TempData["UserProfileViewMessage"] = $"{inputModel.UserName} data has been updated.";
            }

            if (this.TempData.ContainsKey("UserProfileViewMessage"))
            {
                inputModel.SuccessMessage = this.TempData["UserProfileViewMessage"] as string;
            }

            return this.View("EditUser", inputModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] string userId)
        {
            await this._usersService.DeleteUser(userId);
            return this.RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore([FromForm]string userId)
        {
            await this._usersService.RestoreUser(userId);
            return this.Ok();
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
            await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(userModel);

            if (this.ModelState.IsValid)
            {
                if (await this._usersService.UpdateUserPasswordAsync(inputModel.UserId, inputModel.Password))
                {
                    this.TempData["UserProfileViewMessage"] = $"{userModel.UserName} password has been set.";
                }
            }

            return this.RedirectToAction("Update", new { id = inputModel.UserId });
        }

        [AcceptVerbs("Post")]
        public IActionResult VerifyBusinessNumber([FromForm(Name = "Company.Name")] string companyName, [FromForm(Name = "Company.BusinessNumber")] string businessNumber)
        {
            if (string.IsNullOrWhiteSpace(companyName) && !string.IsNullOrEmpty(businessNumber))
            {
                return this.Json("The Business number require Company.");
            }

            return this.Json(true);
        }

        [AcceptVerbs("Post")]
        public IActionResult VerifyCompanyName([FromForm(Name = "Company.Name")] string companyName, [FromForm(Name = "Company.BusinessNumber")] string businessNumber)
        {
            if (string.IsNullOrWhiteSpace(businessNumber) && !string.IsNullOrEmpty(companyName))
            {
                return this.Json("The Company must have Business number.");
            }

            return this.Json(true);
        }

        #region private

        private async Task ApplyMissingPropertiesToEditUserProfileViewComponentModel(UserProfileViewModel userModel)
        {

            var vendors = await this._usersService
                .GetAllInRoleAsync<AddUserDistributorViewModel>(GlobalConstants.Roles.VendorRoleName);

            userModel.AvailableRoles = await this._roleManager.Roles.To<SelectListItem>().ToListAsync();
            userModel.AvailableRoles.ForEach(r => { r.Selected = r.Value == userModel.RoleName; });
            userModel.Company = userModel.Company ?? new UserProfileCompanyViewModel();
            userModel.Company.ContactAddress = userModel.Company.ContactAddress ?? new UserProfileAddressViewModel();
            userModel.Company.ShippingAddress = userModel.Company.ShippingAddress ?? new UserProfileAddressViewModel();
            userModel.ContactAddress = userModel.ContactAddress ?? new UserProfileAddressViewModel();

            if (vendors.Any())
            {
                userModel.AvailableRoles.RemoveAll(r => r.Text == GlobalConstants.Roles.VendorRoleName);
            }

            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = assembly?.GetManifestResourceStream("Vxp.Web.Resources.AllCountriesInTheWorlds.csv");

            if (resourceStream != null)
            {
                var allCountries = new HashSet<SelectListItem>();
                using (var reader = new StreamReader(resourceStream))
                {
                    string country;
                    while ((country = reader.ReadLine()) != null)
                    {
                        allCountries.Add(new SelectListItem(country, country));
                    }
                }

                allCountries.Add(new SelectListItem("- Select Country -", string.Empty, true, true));
                userModel.AvailableCountries = allCountries;
            }
        }

        #endregion
    }
}
