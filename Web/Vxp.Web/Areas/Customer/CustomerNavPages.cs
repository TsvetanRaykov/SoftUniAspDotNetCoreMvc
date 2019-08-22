using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vxp.Web.Areas.Customer
{
    public class CustomerNavPages
    {
        public static string ProductsList => "Index";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, ProductsList);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                                 ?? viewContext.RouteData.Values["Action"].ToString();
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}