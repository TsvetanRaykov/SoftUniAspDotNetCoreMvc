namespace Vxp.Web.Infrastructure.Attributes.ActionFilters
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Extensions;

    public class RestoreModelStateFromTempDataAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is Controller controller && controller.TempData.ContainsKey("ModelState"))
            {
                var tmpDictionary = controller.TempData.Get<Dictionary<string, List<string>>>("ModelState");
                foreach (var state in tmpDictionary)
                {
                    foreach (var error in state.Value)
                    {
                        controller.ViewData.ModelState.AddModelError(state.Key, error);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}