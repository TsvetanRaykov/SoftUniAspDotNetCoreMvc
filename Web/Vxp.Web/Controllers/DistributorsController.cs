using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vxp.Common;
using Vxp.Services.Data.Users;
using Vxp.Web.ViewModels.Components;
using Vxp.Web.ViewModels.Distributors;

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

            var usrDistributors = this._distributorsService
                .GetDistributorsForUser<EditUserProfileViewComponentModelDistributorModel>(customerName).GetAwaiter().GetResult().Select(u=>u.Email).ToHashSet();


            var allDistributors = await this._usersService
                .GetAllInRoleAsync<EditUserProfileViewComponentModelDistributorModel>(GlobalConstants.Roles
                    .DistributorRoleName);

            var availableDistributors = await 
                allDistributors.Where(d => !usrDistributors.Contains(d.Email)).ToArrayAsync();


            return this.Ok(availableDistributors);
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

        [HttpPost]
        public async Task<ActionResult> Connect(DistributorConnectInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                var distributorKey = await this._distributorsService.GenerateNewDistributorKeyAsync(inputModel.DistributorEmail);

                if (await this._distributorsService.AddCustomerToDistributorAsync(inputModel.CustomerEmail, distributorKey))
                {
                    return this.Ok();
                }
            }
            return this.BadRequest();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Disconnect(DistributorConnectInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {

                //var distributorKey = await this._distributorsService.GenerateNewDistributorKeyAsync(inputModel.DistributorEmail);

                if (await this._distributorsService.RemoveCustomerFromDistributorAsync(inputModel.CustomerEmail, inputModel.DistributorEmail))
                {
                    return this.Ok();
                }
            }
            return this.BadRequest();
        }

    }
}