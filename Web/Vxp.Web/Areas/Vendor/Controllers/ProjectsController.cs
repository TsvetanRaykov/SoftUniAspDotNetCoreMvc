namespace Vxp.Web.Areas.Vendor.Controllers
{
    using Infrastructure.Attributes.ActionFilters;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using ViewModels.Projects;
    using Vxp.Services.Data.Projects;

    public class ProjectsController : VendorsController
    {
        private readonly IProjectsService _projectsService;

        public ProjectsController(IProjectsService projectsService)
        {
            this._projectsService = projectsService;
        }

        [RestoreModelStateFromTempData]
        public async Task<IActionResult> Index()
        {
            var viewModel = new ProjectsListViewModel
            {
                ExistingProjects = await this._projectsService
                    .GetAllProjects<ProjectInputModel>(this.User.Identity.Name)
                    .ToListAsync()
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