namespace Vxp.Web.ViewModels.ModelBinders
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    public class FromJsonModelBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            try
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
                if (valueProviderResult != null)
                {
                    var productDetails = JsonConvert.DeserializeObject<T>(valueProviderResult);
                    bindingContext.Result = ModelBindingResult.Success(productDetails);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            catch
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}