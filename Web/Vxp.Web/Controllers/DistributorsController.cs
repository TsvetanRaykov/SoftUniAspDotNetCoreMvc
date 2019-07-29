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
        private readonly IDistributorsService _distributorsService;

        public DistributorsController(IUsersService usersService, IDistributorsService distributorsService)
        {
            this._usersService = usersService;
            this._distributorsService = distributorsService;
        }

        [HttpGet("[action]/{customerName}")]
        public async Task<ActionResult<string>> GetAvailable(string customerName)
        {
            var distributors = await this._distributorsService.GetDistributors<EditUserProfileViewComponentModelDistributorModel>(customerName);
            

            return this.Ok(distributors.ToArray());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var distributors = await
                this._usersService.GetAll<EditUserProfileViewComponentModelDistributorModel>(d => d.Id == id);

            if (!distributors.Any())
            {
                return this.BadRequest();
            }

            return this.Ok(distributors);
        }
    }
}