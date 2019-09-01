namespace Vxp.Web.Areas.Vendor.Controllers
{
    using Common;
    using Vxp.Services.Data.Users;
    using Infrastructure.Attributes.ActionFilters;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using ViewModels.Projects;
    using Vxp.Services.Data.Projects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Vxp.Data.Common.Enums;
    using Vxp.Web.Infrastructure.Extensions;

    public class ProjectsController : VendorsController
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
                    AvailablePartners = await this._usersService.GetAllInRoleAsync<ProjectPartnerInputModel>(GlobalConstants.Roles.DistributorRoleName)
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