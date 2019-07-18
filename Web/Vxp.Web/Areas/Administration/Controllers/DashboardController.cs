namespace Vxp.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Vxp.Services.Data;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService _settingsService;

        public DashboardController(ISettingsService settingsService)
        {
            this._settingsService = settingsService;
        }

        public IActionResult Index()
        {
            return this.RedirectToAction("List", "Users");
        }

        public IActionResult Settings()
        {
            return this.View();
        }
    }
}
