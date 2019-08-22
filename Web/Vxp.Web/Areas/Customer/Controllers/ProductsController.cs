using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vxp.Web.Areas.Customer.Controllers
{
    public class ProductsController : CustomersController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}