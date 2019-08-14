using System.Diagnostics;
using Vxp.Common;
using Vxp.Web.ViewModels;

namespace Vxp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        public IActionResult Index()
        {

            if (!this.User.Identity.IsAuthenticated)
            {
                return this.View();
            }

            if (this.User.IsInRole(GlobalConstants.Roles.AdministratorRoleName))
            {
                return this.RedirectToAction("Index", "Dashboard", new { area = "Administration" });
            }

            if (this.User.IsInRole(GlobalConstants.Roles.VendorRoleName))
            {
                return this.RedirectToAction("Index", "Products", new { area = "Vendor" });
            }

            // TODO: Add redirection for customer, partner and vendor roles

            return this.View();

        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult Technology()
        {
            return this.View();
        }
    }
}
