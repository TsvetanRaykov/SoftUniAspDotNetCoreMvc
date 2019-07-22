using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Data.Models;
using Vxp.Services.Data.Users;
using Vxp.Web.ViewModels.Administration.Users;
using Vxp.Web.ViewModels.Components;

namespace Vxp.Web.ViewComponents
{
    [Authorize]
    public class EditUserProfileViewComponent : ViewComponent
    {
        private readonly IUsersService _usersService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditUserProfileViewComponent(IUsersService usersService,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this._usersService = usersService;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(UserIdInputModel inputModel)
        {
            var userModel = this._usersService
                .GetAllAsync<EditUserProfileViewComponentModel>(u => u.Id == inputModel.UserId)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault();

            if (userModel == null)
            {
                return this.View(new EditUserProfileViewComponentModel());
            }

            userModel.Company = userModel.Company ?? new EditUserProfileViewComponentCompanyModel();
            userModel.Company.ContactAddress =
                userModel.Company.ContactAddress ?? new EditUserProfileViewComponentAddressModel();
            userModel.Company.ShippingAddress =
                userModel.Company.ShippingAddress ?? new EditUserProfileViewComponentAddressModel();

            userModel.ContactAddress = userModel.ContactAddress ?? new EditUserProfileViewComponentAddressModel();
            userModel.AvailableCountries = this._usersService.GetAllCountriesAsync().GetAwaiter().GetResult();

            userModel.AvailableRoles = this._roleManager.Roles
                .Select(role => new SelectListItem(
                    role.Name,
                    role.Id,
                    role.Id == userModel.RoleId,
                    false)).ToList();

            return this.View(userModel);
        }
    }
}