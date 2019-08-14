using Microsoft.AspNetCore.Mvc;

namespace Vxp.Web.Areas.Vendor.Controllers
{
    public class ProductsController : VendorsController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}