namespace Vxp.Web.Areas.Customer.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Common;
    using Vxp.Web.Controllers;

    [Authorize(Roles = GlobalConstants.Roles.CustomerRoleName)]
    [Area("Customer")]
    public class CustomersController : BaseController
    {

    }
}