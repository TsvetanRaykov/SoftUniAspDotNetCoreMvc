namespace Vxp.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Data.Models;
    using Vxp.Services.Data.Users;
    using Services.Mapping;
    using Vxp.Web.ViewModels.Administration.Users;
    using System;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using ViewModels.Users;

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
                RoleName = GlobalConstants.Roles.DistributorRoleName, // Role dropdown selected item by default
                IsNewUser = true,
                UserId = Guid.NewGuid().ToString()
            };

            await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(viewModel);

            return this.View("CreateUser", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfileViewModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(inputModel);


                if (this.ModelState[nameof(inputModel.Password)].ValidationState == ModelValidationState.Invalid)
                {
                    TempData["ErrorMessage"] = this.ModelState[nameof(inputModel.Password)].Errors.FirstOrDefault()?.ErrorMessage;
                }

                return this.View("CreateUser", inputModel);
            }

            var newUserId = await this._usersService.CreateUser(inputModel);

            return this.RedirectToAction(nameof(this.List));
        }

        public async Task<IActionResult> Edit(UserIdInputModel inputModel)
        {

            var userModels = await this._usersService
                .GetAll<UserProfileViewModel>(u => u.Id == inputModel.Id);

            var userModel = userModels.FirstOrDefault();

            userModel = userModel ?? new UserProfileViewModel
            {
                IsNewUser = true
            };

            await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(userModel);

            if (this.TempData.ContainsKey("EditUserViewMessage"))
            {
                userModel.SuccessMessage = this.TempData["EditUserViewMessage"] as string;
            }

            return this.View("EditUser", userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileViewModel inputModel)
        {
            await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(inputModel);

            if (this.ModelState.IsValid && this._usersService.UpdateUser(inputModel))
            {
                this.TempData["EditUserViewMessage"] = $"{inputModel.UserName} data has been updated.";
            }

            if (this.TempData.ContainsKey("EditUserViewMessage"))
            {
                inputModel.SuccessMessage = this.TempData["EditUserViewMessage"] as string;
            }

            return this.View("EditUser", inputModel);
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
                    this.TempData["EditUserViewMessage"] = $"{userModel.UserName} password has been set.";
                }
            }

            return this.RedirectToAction("Edit", new { id = inputModel.UserId });
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

        private async Task ApplyRolesAndDistributorsToAddUserInputModel(AddUserInputModel addUserInputModel)
        {
            var distributors = await this._usersService
                .GetAllInRoleAsync<AddUserDistributorViewModel>(GlobalConstants.Roles.DistributorRoleName);

            var vendors = await this._usersService
                .GetAllInRoleAsync<AddUserDistributorViewModel>(GlobalConstants.Roles.VendorRoleName);


            addUserInputModel.AvailableRoles = this._roleManager.Roles
                .Select(role => new SelectListItem(
                    role.Name,
                    role.Name,
                    role.Name == addUserInputModel.Role,
                    false)).ToList();

            addUserInputModel.AvailableDistributors = distributors.Select(x => new SelectListItem(x.DisplayName, x.Id, x.Id == addUserInputModel.DistributorId)).ToList();
            addUserInputModel.AvailableDistributors.Add(new SelectListItem("- Select Distributor -", string.Empty, addUserInputModel.DistributorId == null, true));
            addUserInputModel.AvailableCountries = this._usersService.GetAllCountriesAsync().GetAwaiter().GetResult();

            if (vendors.Any())
            {
                addUserInputModel.AvailableRoles.RemoveAll(r => r.Text == GlobalConstants.Roles.VendorRoleName);
            }
        }

        private async Task ApplyMissingPropertiesToEditUserProfileViewComponentModel(UserProfileViewModel userModel)
        {
            userModel.AvailableCountries = await this._usersService.GetAllCountriesAsync();
            userModel.AvailableRoles = await this._roleManager.Roles.To<SelectListItem>().ToListAsync();
            userModel.AvailableRoles.ForEach(r => { r.Selected = r.Value == userModel.RoleName; });
            userModel.Company = userModel.Company ?? new EditUserProfileCompanyViewModel();
            userModel.Company.ContactAddress = userModel.Company.ContactAddress ?? new EditUserProfileAddressViewModel();
            userModel.Company.ShippingAddress = userModel.Company.ShippingAddress ?? new EditUserProfileAddressViewModel();
            userModel.ContactAddress = userModel.ContactAddress ?? new EditUserProfileAddressViewModel();
        }

        #endregion
    }
}
