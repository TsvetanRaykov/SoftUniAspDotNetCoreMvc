using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vxp.Web.Areas.Vendor
{
    public static class VendorNavPages
    {
        public static string Index => "Index";
        public static string AddProduct => "Create";
        public static string ManageCategories => "Categories";
        public static string ManageDetails => "Details";
        public static string DeletedProducts => "Deleted";

        public static string DistributorsList => "DistributorsList";
        public static string DistributorsRegister => "DistributorsRegister";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        public static string DeletedProductsNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletedProducts);
        public static string AddProductNavClass(ViewContext viewContext) => PageNavClass(viewContext, AddProduct);
        public static string CategoriesNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageCategories);
        public static string DetailsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ManageDetails);
        public static string DistributorsListNavClass(ViewContext viewContext) => PageNavClass(viewContext, DistributorsList);
        public static string DistributorsRegisterNavClass(ViewContext viewContext) => PageNavClass(viewContext, DistributorsRegister);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? viewContext.RouteData.Values["Action"].ToString();
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}