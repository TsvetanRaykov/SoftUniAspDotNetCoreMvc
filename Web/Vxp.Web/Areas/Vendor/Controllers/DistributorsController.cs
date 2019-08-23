using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vxp.Common;
using Vxp.Services.Data.Products;
using Vxp.Services.Data.Users;
using Vxp.Web.ViewModels.Prices;
using Vxp.Web.ViewModels.Users;
using Vxp.Web.ViewModels.Vendor.Distributors;

namespace Vxp.Web.Areas.Vendor.Controllers
{
    public class DistributorsController : VendorsController
    {
        private readonly IUsersService _usersService;
        private readonly IProductPricesService _productsPricesService;

        public DistributorsController(IUsersService usersService, IProductPricesService productsPricesService)
        {
            this._usersService = usersService;
            this._productsPricesService = productsPricesService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await this._usersService.GetAllInRoleAsync<DistributorsListViewModel>(
                    GlobalConstants.Roles.DistributorRoleName);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePriceModel(PriceModifierInputModel inputModel)
        {

            if (this.ModelState.IsValid)
            {
                await this._productsPricesService.SetProductPriceModifierAsync(inputModel);
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Register()
        {
            var viewModel = new UserProfileInputModel
            {
                IsNewUser = true,
                RoleName = GlobalConstants.Roles.DistributorRoleName
            };
            await this._usersService.PopulateCommonUserModelPropertiesAsync(viewModel);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserProfileInputModel inputModel)
        {
            if (string.IsNullOrEmpty(inputModel.ContactAddress?.CountryName))
            {
                this.ModelState.AddModelError("ContactAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "ContactAddress"));
            }

            if (string.IsNullOrEmpty(inputModel.Company?.ShippingAddress?.CountryName))
            {
                this.ModelState.AddModelError("Company.ShippingAddress.CountryName", string.Format(GlobalConstants.ErrorMessages.RequiredField, "ShippingAddress"));
            }

            if (this.ModelState.IsValid)
            {

                var newUserId = await this._usersService.CreateUserAsync(inputModel, inputModel.Password,
                    GlobalConstants.Roles.DistributorRoleName);

                if (!string.IsNullOrEmpty(newUserId))
                {
                    this.TempData["UserProfileViewMessage"] = $"{inputModel.Company?.Name} has been created.";
                }
                else
                {
                    this.TempData["ErrorMessage"] = $"{inputModel.Company?.Name} cannot be created. Please, contact support.";
                }

                return this.RedirectToAction(nameof(this.Edit), new { id = newUserId });
            }

            await this._usersService.PopulateCommonUserModelPropertiesAsync(inputModel);
            return this.View(inputModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(string id)
        {
            await this._usersService.RestoreUserAsync(id);
            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var userModel = await this._usersService
                .GetAll<UserProfileInputModel>(u => u.Id == id).GetAwaiter().GetResult()
                .FirstOrDefaultAsync();

            if (userModel == null)
            {
                return this.BadRequest();
            }

            await this._usersService.PopulateCommonUserModelPropertiesAsync(userModel);

            if (this.TempData.ContainsKey("UserProfileViewMessage"))
            {
                userModel.StatusMessage = this.TempData["UserProfileViewMessage"] as string;
            }

            return this.View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await this._usersService.DeleteUserAsync(userId);
            return this.RedirectToAction(nameof(this.Index));
        }

    }
}