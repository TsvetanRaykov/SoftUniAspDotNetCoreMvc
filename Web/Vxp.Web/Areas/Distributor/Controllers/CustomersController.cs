using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    public class CustomersController : DistributorsController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Invite()
        {
            return this.View();
        }

        public IActionResult Pending()
        {
            return this.View();
        }
    }
}