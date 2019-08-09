using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Web.ViewModels.ModelBinders
{
    public class TransparentPropertyModelBinder : IModelBinder
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
                    bindingContext.ValueProvider.GetValue(bindingContext.FieldName).FirstOrDefault();

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