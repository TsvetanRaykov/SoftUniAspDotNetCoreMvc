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
            var roles = this._usersService.GetAllRoles().ToDictionary(x => x.Key, x => x.Value);

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
            var distributors = await this._usersService.GetAllInRoleAsync<AddUserDistributorViewModel>(GlobalConstants.Roles.PartnerRoleName);

            var viewModel = new AddUserInputModel
            {
                Roles = this._usersService.GetAllRoles().ToDictionary(x => x.Key, x => x.Value),
                Distributors = distributors.ToDictionary(x => x.Id, x => x.DisplayName)
            };

            return this.View(viewModel);
        }

        public IActionResult Settings()
        {
            return this.View();
        }
    }
}
