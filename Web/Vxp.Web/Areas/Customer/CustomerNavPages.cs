using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vxp.Web.Areas.Customer
{
    public class CustomerNavPages
    {
        public static string ProductsList => "Index";
        public static string DistributorsList => "DistributorsList";
        public static string ProjectsList => "ProjectsList";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, ProductsList);
        public static string DistributorsListNavClass(ViewContext viewContext) => PageNavClass(viewContext, DistributorsList);
        public static string ProjectsListNavClass(ViewContext viewContext) => PageNavClass(viewContext, ProjectsList);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                                 ?? viewContext.RouteData.Values["Action"].ToString();
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}