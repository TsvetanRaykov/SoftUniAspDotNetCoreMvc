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

            inputModel.UploadInputModel.ProjectId = inputModel.Id;

            return this.View(inputModel);
        }
    }
}