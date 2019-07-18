using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Common;
using Vxp.Data.Models;
using Vxp.Services.Data;
using Vxp.Web.ViewModels.Administration.Users;


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

            var viewModel = await this._usersService.GetAllAsync<ListUserViewModel>();

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
                Role = GlobalConstants.Roles.VendorRoleName // Role dropdown selected item
            };

            await this.ApplyRolesAndDistributorsToAddUserInputModel(viewModel);

            return this.View("CreateUser", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddUserInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                await this.ApplyRolesAndDistributorsToAddUserInputModel(inputModel);

                return this.View("CreateUser", inputModel);
            }

            string newUserId = await this._usersService.CreateUser(inputModel);

            return this.RedirectToAction(nameof(this.List));
        }


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
            addUserInputModel.AvailableCountries = this._usersService.GetAllCountries().GetAwaiter().GetResult();

            if (vendors.Any())
            {
                addUserInputModel.AvailableRoles.RemoveAll(r => r.Text == GlobalConstants.Roles.VendorRoleName);
            }
        }

    }
}
