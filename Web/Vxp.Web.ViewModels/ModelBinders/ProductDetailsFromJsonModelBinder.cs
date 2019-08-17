using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Vxp.Web.ViewModels.Products;

namespace Vxp.Web.ViewModels.ModelBinders
{
    public class ProductDetailsFromJsonModelBinder :IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            try
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                var productDetails = JsonConvert.DeserializeObject<List<ProductDetailInputModel>>(valueProviderResult.FirstValue);
                
                bindingContext.Result = ModelBindingResult.Success(productDetails);
            }
            catch
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}