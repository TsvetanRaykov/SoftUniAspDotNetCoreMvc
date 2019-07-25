using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Vxp.Web.Infrastructure.ModelBinders
{
    public class BankAccountToSelectListItemModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(IEnumerable<SelectListItem>))
            {
                return new BinderTypeModelBinder(typeof(BankAccountToSelectListItemModelBinder));
            }

            return null;
        }
    }
}