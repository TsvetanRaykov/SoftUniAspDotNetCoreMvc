using System.Linq;
using System.Threading.Tasks;
using Vxp.Data;

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
            _settingsService = settingsService;
            _usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {

            var roles = this._usersService.GetAllRoles().ToDictionary(x => x.Key, x => x.Value);

            var viewModel = new IndexViewModel
            {
                SettingsCount = this._settingsService.GetCount(),
                Users = await this._usersService.GetAllAsync<UserViewModel>()
            };

            foreach (var viewModelUser in viewModel.Users)
            {
                viewModelUser.Role = roles[viewModelUser.Roles.First().RoleId];
            }

            return this.View(viewModel);
        }
    }
}
