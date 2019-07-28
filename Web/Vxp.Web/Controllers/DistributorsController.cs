using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Vxp.Services.Data.Users;
using Vxp.Web.ViewModels.Components;

namespace Vxp.Web.Controllers
{
    public class DistributorsController : ApiController
    {
        private readonly IUsersService _usersService;

        public DistributorsController(IUsersService usersService)
        {
            this._usersService = usersService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var distributor = await
                this._usersService.GetAll<EditUserProfileViewComponentModelDistributorModel>(d => d.Id == id);

            return this.Ok(distributor.FirstOrDefault());
        }
    }
}