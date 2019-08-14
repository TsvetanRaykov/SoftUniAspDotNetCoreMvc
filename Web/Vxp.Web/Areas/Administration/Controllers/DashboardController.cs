using Vxp.Services.Data.BankAccounts;

namespace Vxp.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using ViewModels.Users;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Vxp.Services.Data.Settings;
    using Vxp.Services.Data.Users;
    using Vxp.Web.ViewModels.Administration.Users;

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
                .GetAll<UserProfileViewModel>(u => u.Id == inputModel.Id);

            var userModel = userModels.FirstOrDefault();

            userModel = userModel ?? new UserProfileViewModel
            {
                IsNewUser = true
            };

            await this._usersService.PopulateCommonUserModelProperties(userModel);

            if (this.TempData.ContainsKey("UserProfileViewMessage"))
            {
                userModel.StatusMessage = this.TempData["UserProfileViewMessage"] as string;
            }

            return this.View(userModel);
        }

        public async Task<IActionResult> CreateUser()
        {

            var viewModel = new UserProfileViewModel
            {
                RoleName = GlobalConstants.Roles.AdministratorRoleName, // Role dropdown selected item by default
                IsNewUser = true,
            };

            await this._usersService.PopulateCommonUserModelProperties(viewModel);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserProfileViewModel inputModel)
        {

            if (string.IsNullOrEmpty(inputModel.ContactAddress?.CountryName))
            {
                this.ModelState.AddModelError("ContactAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "CountryName"));
            }

            if (!this.ModelState.IsValid)
            {
                await this._usersService.PopulateCommonUserModelProperties(inputModel);


                if (this.ModelState[nameof(inputModel.Password)].ValidationState == ModelValidationState.Invalid)
                {
                    this.TempData["ErrorMessage"] = this.ModelState[nameof(inputModel.Password)].Errors.FirstOrDefault()?.ErrorMessage;
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
                this.TempData["ErrorMessage"] = $"{inputModel.UserName} cannot be created.";
            }
            return this.RedirectToAction(nameof(this.EditUser), new { id = newUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserProfileViewModel inputModel)
        {
            await this._usersService.PopulateCommonUserModelProperties(inputModel);

            if (string.IsNullOrEmpty(inputModel.ContactAddress?.CountryName))
            {
                this.ModelState.AddModelError("ContactAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "CountryName"));
            }

            if (inputModel.RoleName == GlobalConstants.Roles.DistributorRoleName || inputModel.RoleName == GlobalConstants.Roles.VendorRoleName)
            {
                var availableBankAccount = this._bankAccountsService.GetAllBankAccounts<UserProfileBankAccountViewModel>()
                    .FirstOrDefault(b => b.OwnerId == inputModel.UserId);
                if (availableBankAccount == null)
                {
                    var errorMessage = string.Format(GlobalConstants.ErrorMessages.RequiredField, "Bank account");
                    this.TempData["ErrorMessage"] = errorMessage;
                    this.ModelState.AddModelError("BankAccount", errorMessage);
                }
            }

            if (this.ModelState.IsValid && await this._usersService.UpdateUser(inputModel, new[] { inputModel.RoleName }))
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
            await this._usersService.DeleteUser(userId);
            return this.RedirectToAction("ListUsers", "Dashboard", new { area = "Administration" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreUser([FromForm]string userId)
        {
            await this._usersService.RestoreUser(userId);
            return this.Ok();
        }


        public IActionResult Settings()
        {
            return this.View();
        }
    }
}
