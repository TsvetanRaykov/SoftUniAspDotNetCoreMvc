using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Common;

namespace Vxp.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Dashboard;
    using Vxp.Services.Data;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService _settingsService;
        private readonly IUsersService _usersService;


        public DashboardController(ISettingsService settingsService, IUsersService usersService)
        {
            this._settingsService = settingsService;
            this._usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var roles = this._usersService.GetAllRoles().ToDictionary(x => x.Value, x => x.Key);

            var viewModel = new IndexViewModel
            {
                SettingsCount = this._settingsService.GetCount(),
                Users = await this._usersService.GetAllAsync<ListUserViewModel>()
            };

            foreach (var viewModelUser in viewModel.Users)
            {
                viewModelUser.Role = roles[viewModelUser.Roles.First().RoleId];
            }

            return this.View(viewModel);
        }

        public async Task<IActionResult> AddUser()
        {
            var distributors = await this._usersService
                .GetAllInRoleAsync<AddUserDistributorViewModel>(GlobalConstants.Roles.PartnerRoleName);

            var vendors = await this._usersService
                .GetAllInRoleAsync<AddUserDistributorViewModel>(GlobalConstants.Roles.VendorRoleName);

            var viewModel = new AddUserInputModel
            {
                Roles = this._usersService.GetAllRoles()
                    .Select(x => new SelectListItem(
                        x.Key,
                        x.Value,
                        x.Key == GlobalConstants.Roles.AdministratorRoleName,
                        false)).ToList(),

                Distributors = distributors
                    .Select(x => new SelectListItem(x.DisplayName, x.Id)).ToList(),

                AvailableCountries = this._usersService.GetAllCountries().GetAwaiter().GetResult()
            };

            if (vendors.Any())
            {
                viewModel.Roles.RemoveAll(r => r.Text == GlobalConstants.Roles.VendorRoleName);
            }

            viewModel.Distributors.Add(new SelectListItem("- Select Distributor -", string.Empty, true, true));
            return this.View(viewModel);
        }

        public IActionResult Settings()
        {
            return this.View();
        }
    }
}
