namespace Vxp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using Common;
    using ViewModels;

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

            if (this.User.IsInRole(GlobalConstants.Roles.DistributorRoleName))
            {
                return this.RedirectToAction("Index", "Products", new { area = "Distributor" });
            }

            if (this.User.IsInRole(GlobalConstants.Roles.CustomerRoleName))
            {
                return this.RedirectToAction("Index", "Products", new { area = "Customer" });
            }

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
