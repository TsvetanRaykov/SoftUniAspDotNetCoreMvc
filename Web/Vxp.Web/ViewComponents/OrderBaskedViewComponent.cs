namespace Vxp.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using Infrastructure.Extensions;

    public class OrderBaskedViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var currentOrder = this.HttpContext.Session.Get<List<int>>("order") ?? new List<int>();

            return this.View(currentOrder.Count);
        }

    }
}