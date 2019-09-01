using System;
using System.Collections.Generic;
using System.Linq;
using Vxp.Data.Common.Enums;
using Vxp.Web.Infrastructure.Extensions;

namespace Vxp.Web.Areas.Customer.Controllers
{
    using Vxp.Services.Data.Users;
    using Infrastructure.Attributes.ActionFilters;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Vxp.Services.Data.Projects;
    using ViewModels.Projects;

    public class ProjectsController : CustomersController
    {
        private readonly IProjectsService _projectsService;
        private readonly IDistributorsService _distributorsService;

        public ProjectsController(IProjectsService projectsService, IDistributorsService distributorsService)
        {
            this._projectsService = projectsService;
            this._distributorsService = distributorsService;
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
                    AvailablePartners = await this._distributorsService.GetDistributorsForUserAsync<ProjectPartnerInputModel>(this.User.Identity.Name)
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