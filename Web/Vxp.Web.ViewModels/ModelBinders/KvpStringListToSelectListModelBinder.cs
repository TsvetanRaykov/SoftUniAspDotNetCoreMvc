using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vxp.Web.ViewModels.ModelBinders
{
    public class KvpStringListToSelectListModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            try
            {
                var valueProviderResult =
                    bindingContext.ValueProvider.GetValue(bindingContext.ModelName)
                        .Select(d => new SelectListItem
                        {
                            Value = d.Split(':')[0],
                            Text = d.Split(':')[1]
                        })
                        .ToArray();

                bindingContext.Result = ModelBindingResult.Success(valueProviderResult);
            }
            catch
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
