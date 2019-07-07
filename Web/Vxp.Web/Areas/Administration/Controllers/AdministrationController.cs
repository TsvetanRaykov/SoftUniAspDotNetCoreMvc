namespace Vxp.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Vxp.Common;
    using Vxp.Web.Controllers;

    [Authorize(Roles = GlobalConstants.Roles.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
    }
}
