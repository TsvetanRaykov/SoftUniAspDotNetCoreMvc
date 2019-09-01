namespace Vxp.Web.Areas.Distributor.Controllers
{
    using System;
    using System.Linq;
    using Vxp.Common;
    using Vxp.Data.Common.Enums;
    using Vxp.Services.Data.Users;
    using Vxp.Web.Infrastructure.Extensions;
    using Infrastructure.Attributes.ActionFilters;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Vxp.Services.Data.Projects;
    using ViewModels.Projects;
    using System.Collections.Generic;

    public class ProjectsController : DistributorsController
    {
        private readonly IProjectsService _projectsService;
        private readonly IUsersService _usersService;

        public ProjectsController(IProjectsService projectsService, IUsersService usersService)
        {
            this._projectsService = projectsService;
            this._usersService = usersService;
        }

        [RestoreModelStateFromTempData]
        public async Task<IActionResult> Index()
        {
            var viewModel = new ProjectsListViewModel
            {
                ExistingProjects = await this._projectsService
                    .GetAllProjects<ProjectInputModel>(this.User.Identity.Name)
                    .ToListAsync(),
                Input = new ProjectInputModel
                {
                    AvailablePartners = await this._usersService.GetAllInRoleAsync<ProjectPartnerInputModel>(GlobalConstants.Roles.VendorRoleName)
                        .GetAwaiter().GetResult().ToListAsync()
                }
            };

            return this.View(viewModel);
        }

        [RestoreModelStateFromTempData]
        public async Task<IActionResult> Project(int id)
        {
            var inputModel = await this._projectsService
                .GetAllProjects<ProjectInputModel>(this.User.Identity.Name)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (inputModel == null)
            {
                return this.NotFound();
            }

            foreach (var order in inputModel.Orders)
            {
                foreach (var product in order.Products)
                {
                    var priceModifierData = product.PriceModifierData.ToObject<Dictionary<string, string>>();

                    Enum.TryParse(priceModifierData[nameof(product.PriceModifierType)], true,
                        out PriceModifierType modifierType);

                    decimal.TryParse(priceModifierData[nameof(product.ModifierValue)], out var modifierValue);

                    product.ModifierValue = modifierValue;
                    product.PriceModifierType = modifierType;
                }
            }

            inputModel.UploadInputModel.ProjectId = inputModel.Id;
            inputModel.Orders = inputModel.Orders.OrderByDescending(o => o.ModifiedOn).ToList();

            return this.View(inputModel);
        }
    }
}