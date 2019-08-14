using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vxp.Common;
using Vxp.Web.Controllers;

namespace Vxp.Web.Areas.Vendor.Controllers
{
    [Authorize(Roles = GlobalConstants.Roles.VendorRoleName)]
    [Area("Vendor")]
    public class VendorsController : BaseController
    {
    }
}