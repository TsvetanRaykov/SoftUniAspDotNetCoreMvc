namespace Vxp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Common;
    using Vxp.Services.Data.Users;
    using ViewModels.Users;
    using Vxp.Services.Data.BankAccounts;
    using Services.Models;
    using ViewModels.Distributor;

    public class DistributorsController : ApiController
    {
        private readonly IUsersService _usersService;
        private readonly IDistributorsService _distributorsService;
        private readonly IBankAccountsService _bankAccountsService;

        public DistributorsController(
            IUsersService usersService, 
            IDistributorsService distributorsService, 
            IBankAccountsService bankAccountsService)
        {
            this._usersService = usersService;
            this._distributorsService = distributorsService;
            this._bankAccountsService = bankAccountsService;
        }

        [HttpGet("[action]/{customerName}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<string>> GetAvailable(string customerName)
        {

            var usrDistributors = this._distributorsService
                .GetDistributorsForUserAsync<UserProfileDistributorViewModel>(customerName).GetAwaiter().GetResult().Select(u => u.UserName).ToHashSet();

            var allDistributors = await this._usersService
                .GetAllInRoleAsync<UserProfileDistributorViewModel>(GlobalConstants.Roles
                    .DistributorRoleName);

            var availableDistributors = await allDistributors
                    .Where(d => !usrDistributors.Contains(d.UserName))
                    .Where(d => d.UserName != customerName)
                    .ToArrayAsync();

            return this.Ok(availableDistributors);
        }

        [HttpGet("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<string>> Get(string id)
        {
            var distributors = await
                this._usersService.GetAll<UserProfileDistributorViewModel>(d => d.Id == id);

            if (!distributors.Any())
            {
                return this.BadRequest();
            }

            return this.Ok(distributors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Connect(DistributorConnectInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                var distributorKey = this._distributorsService.GenerateNewKeyForDistributor(inputModel.DistributorEmail);
                var bankAccount = this._bankAccountsService
                    .GetBankAccountsForUser<BankAccountDto>(inputModel.DistributorEmail).FirstOrDefault();
                if (bankAccount == null)
                {
                    return this.BadRequest();
                }

                await this._bankAccountsService.AssignDistributorKeyToBankAccountAsync(bankAccount.Id, distributorKey);

                if (await this._distributorsService.AddCustomerToDistributorAsync(inputModel.CustomerEmail, distributorKey))
                {
                    return this.Ok();
                }
            }
            return this.BadRequest();
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disconnect(DistributorConnectInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                if (await this._distributorsService.RemoveCustomerFromDistributorAsync(inputModel.CustomerEmail, inputModel.DistributorEmail))
                {
                    return this.Ok();
                }
            }
            return this.BadRequest();
        }

    }
}