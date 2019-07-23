using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Services.Data.Users;
using Vxp.Web.ViewModels.Administration.Users;
using Vxp.Web.ViewModels.Components;

namespace Vxp.Web.ViewComponents
{
    [Authorize]
    public class EditUserProfileViewComponent : ViewComponent
    {
        private readonly IUsersService _usersService;
        private readonly IRolesService _rolesService;

        public EditUserProfileViewComponent(IUsersService usersService, IRolesService rolesService)
        {
            this._usersService = usersService;
            this._rolesService = rolesService;
        }
        public async Task<IViewComponentResult> InvokeAsync(UserIdInputModel inputModel)
        {
            var userModels = await this._usersService
                .GetAll<EditUserProfileViewComponentModel>(u => u.Id == inputModel.UserId)
                .ToListAsync();

            var userModel = userModels.FirstOrDefault();

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

            var roles = this._rolesService.GetAll<SelectListItem>();
            var userRole = roles.FirstOrDefault(r => r.Value == userModel.RoleId);
            if (userRole != null)
            {
                userRole.Selected = true;
            }

            userModel.AvailableRoles = roles.ToList();

            return this.View(userModel);
        }
    }
}