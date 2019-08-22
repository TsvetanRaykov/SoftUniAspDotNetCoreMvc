using Microsoft.AspNetCore.Authorization;

namespace Vxp.Web.Areas.Administration.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels.Users;
    using Vxp.Services.Data.Users;
    using Vxp.Services.Data.BankAccounts;
    using Vxp.Web.ViewModels.Administration.Dashboard;

    public class DashboardController : AdministrationController
    {
        private readonly IUsersService _usersService;
        private readonly IBankAccountsService _bankAccountsService;

        public DashboardController(IUsersService usersService, IBankAccountsService bankAccountsService)
        {
            this._usersService = usersService;
            this._bankAccountsService = bankAccountsService;
        }

        public IActionResult Index()
        {
            return this.RedirectToAction(nameof(this.ListUsers));
        }

        public async Task<IActionResult> ListUsers()
        {
            var viewModel = await this._usersService.GetAllWithDeleted<ListUserViewModel>().GetAwaiter().GetResult().ToListAsync();
            return this.View(viewModel);
        }

        public async Task<IActionResult> EditUser(UserIdInputModel inputModel)
        {
            var userModels = await this._usersService
                .GetAll<UserProfileInputModel>(u => u.Id == inputModel.Id);

            var userModel = userModels.FirstOrDefault();

            userModel = userModel ?? new UserProfileInputModel
            {
                IsNewUser = true
            };

            await this._usersService.PopulateCommonUserModelPropertiesAsync(userModel);

            if (this.TempData.ContainsKey("UserProfileViewMessage"))
            {
                userModel.StatusMessage = this.TempData["UserProfileViewMessage"] as string;
            }

            return this.View(userModel);
        }

        public async Task<IActionResult> CreateUser()
        {

            var viewModel = new UserProfileInputModel
            {
                RoleName = GlobalConstants.Roles.AdministratorRoleName, // Role dropdown selected item by default
                IsNewUser = true,
                IsEmailConfirmed = true
            };

            await this._usersService.PopulateCommonUserModelPropertiesAsync(viewModel);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserProfileInputModel inputModel)
        {

            if (string.IsNullOrEmpty(inputModel.ContactAddress?.CountryName))
            {
                this.ModelState.AddModelError("ContactAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "CountryName"));
            }

            if (!this.ModelState.IsValid)
            {
                if (this.ModelState[nameof(inputModel.Password)].ValidationState == ModelValidationState.Invalid)
                {
                    this.TempData["ErrorMessage"] = this.ModelState[nameof(inputModel.Password)].Errors.FirstOrDefault()?.ErrorMessage;
                }

                await this._usersService.PopulateCommonUserModelPropertiesAsync(inputModel);
                return this.View("CreateUser", inputModel);
            }

            var newUserId = await this._usersService.CreateUserAsync(inputModel, inputModel.Password, inputModel.RoleName);

            if (!string.IsNullOrEmpty(newUserId))
            {
                this.TempData["UserProfileViewMessage"] = $"{inputModel.UserName} has been created.";
            }
            else
            {
                this.TempData["ErrorMessage"] = $"{inputModel.UserName} cannot be created. Please, contact support.";
            }
            return this.RedirectToAction(nameof(this.EditUser), new { id = newUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserProfileInputModel inputModel)
        {
            await this._usersService.PopulateCommonUserModelPropertiesAsync(inputModel);

            if (string.IsNullOrEmpty(inputModel.ContactAddress?.CountryName))
            {
                this.ModelState.AddModelError("ContactAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "CountryName"));
            }

            if (inputModel.RoleName == GlobalConstants.Roles.DistributorRoleName || inputModel.RoleName == GlobalConstants.Roles.VendorRoleName)
            {
                var availableBankAccount = this._bankAccountsService
                    .GetBankAccountsForUser<UserProfileBankAccountInputModel>(inputModel.UserName)
                    .FirstOrDefault();
                if (availableBankAccount == null)
                {
                    var errorMessage = string.Format(GlobalConstants.ErrorMessages.RequiredField, "Bank account");
                    this.TempData["ErrorMessage"] = errorMessage;
                    this.ModelState.AddModelError("BankAccount", errorMessage);
                }
            }

            if (this.ModelState.IsValid && await this._usersService.UpdateUserAsync(inputModel, new[] { inputModel.RoleName }))
            {
                this.TempData["UserProfileViewMessage"] = $"{inputModel.UserName} data has been updated.";
            }

            if (this.TempData.ContainsKey("UserProfileViewMessage"))
            {
                inputModel.StatusMessage = this.TempData["UserProfileViewMessage"] as string;
            }

            return this.View("EditUser", inputModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser([FromForm] string userId)
        {
            await this._usersService.DeleteUserAsync(userId);
            return this.RedirectToAction("ListUsers", "Dashboard", new { area = "Administration" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreUser([FromForm]string userId)
        {
            await this._usersService.RestoreUserAsync(userId);
            return this.Ok();
        }


        public IActionResult Settings()
        {
            return this.View();
        }
    }
}
