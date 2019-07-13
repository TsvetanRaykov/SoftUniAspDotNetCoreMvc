namespace Vxp.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Vxp.Services.Data;
    using ViewModels.Dashboard;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService _settingsService;

        public DashboardController(ISettingsService settingsService)
        {
            this._settingsService = settingsService;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel { SettingsCount = this._settingsService.GetCount(), };
            return this.View(viewModel);
        }
    }
}
