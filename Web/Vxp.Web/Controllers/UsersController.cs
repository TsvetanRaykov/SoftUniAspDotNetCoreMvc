using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vxp.Web.ViewModels.Administration.Users;

namespace Vxp.Web.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(VxpUserInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("EditUser");
            }
            return this.LocalRedirect(inputModel.ReturnUrl);
        }
    }
}