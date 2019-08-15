using Microsoft.AspNetCore.Mvc;

namespace Vxp.Web.Areas.Vendor.Controllers
{
    public class ProductsController : VendorsController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return this.View();
        }
        public IActionResult Categories()
        {
            return this.View();
        }
        public IActionResult Details()
        {
            return this.View();
        }
    }
}