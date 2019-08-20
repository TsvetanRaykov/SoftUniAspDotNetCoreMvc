using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vxp.Web.Areas.Distributor
{
    public static class DistributorNavPages
    {
        public static string ProductsList => "Index";
        public static string Invite => "Invite";
        public static string Pending => "Pending";
        public static string CustomersList => "CustomersList";


        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, ProductsList);
        public static string InviteNavClass(ViewContext viewContext) => PageNavClass(viewContext, Invite);
        public static string PendingNavClass(ViewContext viewContext) => PageNavClass(viewContext, Pending);
        public static string CustomersNavClass(ViewContext viewContext) => PageNavClass(viewContext, CustomersList);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? viewContext.RouteData.Values["Action"].ToString();
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}