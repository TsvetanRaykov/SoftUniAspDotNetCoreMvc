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

            inputModel.UploadInputModel.ProjectId = inputModel.Id;

            return this.View(inputModel);
        }
    }
}