using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vxp.Common;
using Vxp.Data.Models;
using Vxp.Services.Data.Users;
using Vxp.Services.Mapping;
using Vxp.Web.ViewModels.Administration.Users;
using Vxp.Web.ViewModels.Components;


namespace Vxp.Web.Areas.Administration.Controllers
{
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
            var roles = this._roleManager.Roles.ToDictionary(x => x.Id, x => x.Name);

            var viewModel = await this._usersService.GetAll<ListUserViewModel>().ToListAsync();

            foreach (var viewModelUser in viewModel)
            {
                viewModelUser.Role = roles[viewModelUser.Roles.First().RoleId];
            }

            return this.View("ListUsers", viewModel);
        }

        public async Task<IActionResult> Create()
        {

            var viewModel = new AddUserInputModel
            {
                Role = GlobalConstants.Roles.VendorRoleName // RoleId dropdown selected item
            };

            await this.ApplyRolesAndDistributorsToAddUserInputModel(viewModel);

            return this.View("CreateUser", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                await this.ApplyRolesAndDistributorsToAddUserInputModel(inputModel);

                return this.View("CreateUser", inputModel);
            }

            var newUserId = await this._usersService.CreateUser(inputModel);

            return this.RedirectToAction(nameof(this.List));
        }

        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserIdInputModel inputModel)
        {

            var userModels = await this._usersService
                .GetAll<EditUserProfileViewComponentModel>(u => u.Id == inputModel.Id)
                .ToListAsync();

            var userModel = userModels.FirstOrDefault();

            userModel = userModel ?? new EditUserProfileViewComponentModel();

            await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(userModel);
            
            return this.View("EditUser", userModel);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserProfileViewComponentModel inputModel)
        {

            if (!this.ModelState.IsValid)
            {
                await this.ApplyMissingPropertiesToEditUserProfileViewComponentModel(inputModel);
                return this.View("EditUser", inputModel);
            }

            return this.Ok();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Update(EditUserProfileViewComponentModel profileViewComponentModel)
        //{
        //    if (!this.ModelState.IsValid)
        //    {
        //        return this.View("EditUser");
        //    }

        //    return this.View("EditUser");
        //}

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

        private async Task ApplyMissingPropertiesToEditUserProfileViewComponentModel(EditUserProfileViewComponentModel userModel)
        {
            userModel.AvailableCountries = await this._usersService.GetAllCountriesAsync();
            userModel.AvailableRoles = await this._roleManager.Roles.To<SelectListItem>().ToListAsync();
            userModel.AvailableRoles.ForEach(r => { r.Selected = r.Value == userModel.RoleId; });
            userModel.Company = userModel.Company ?? new EditUserProfileViewComponentCompanyModel();
            userModel.Company.ContactAddress = userModel.Company.ContactAddress ?? new EditUserProfileViewComponentAddressModel();
            userModel.Company.ShippingAddress = userModel.Company.ShippingAddress ?? new EditUserProfileViewComponentAddressModel();
            userModel.ContactAddress = userModel.ContactAddress ?? new EditUserProfileViewComponentAddressModel();
        }
    }
}
