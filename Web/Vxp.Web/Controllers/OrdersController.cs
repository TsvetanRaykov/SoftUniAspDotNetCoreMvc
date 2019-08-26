using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vxp.Web.Infrastructure.Extensions;

namespace Vxp.Web.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProductToOrder(int productId)
        {

            var currentOrder = this.HttpContext.Session.Get<List<int>>("order") ?? new List<int>();

            currentOrder.Add(productId);
            this.HttpContext.Session.Set("order", currentOrder);

            return this.Ok(currentOrder.Count);
        }
    }
}