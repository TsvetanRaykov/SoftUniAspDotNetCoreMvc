namespace Vxp.Web.Infrastructure.Attributes.ActionFilters
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Extensions;

    public class SetTempDataModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller is Controller controller && !controller.ModelState.IsValid)
            {
                var tmpDictionary = new Dictionary<string, List<string>>();
                foreach (var modelState in controller.ModelState)
                {
                    tmpDictionary.Add(modelState.Key, new List<string>());
                    foreach (var error in modelState.Value.Errors)
                    {
                        tmpDictionary[modelState.Key].Add(error.ErrorMessage);
                    }
                }
                controller.TempData.Put("ModelState", tmpDictionary);
            }

            base.OnActionExecuted(filterContext);
        }
    }
}