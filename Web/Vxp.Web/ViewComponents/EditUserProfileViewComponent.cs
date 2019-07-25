using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vxp.Web.ViewModels.Components;

namespace Vxp.Web.ViewComponents
{
    [Authorize]
    public class EditUserProfileViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(EditUserProfileViewComponentModel inputModel)
        {
            return this.View(inputModel);
        }
    }
}