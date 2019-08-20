namespace Vxp.Web.Areas.Distributor.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Common;
    using Vxp.Web.Controllers;

    [Authorize(Roles = GlobalConstants.Roles.DistributorRoleName)]
    [Area("Distributor")]
    public class DistributorsController : BaseController
    {
    }
}