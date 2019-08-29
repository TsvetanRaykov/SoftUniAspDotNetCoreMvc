using System.Linq;
using Vxp.Common;
using Vxp.Services.Data.Users;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    using Infrastructure.Attributes.ActionFilters;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Vxp.Services.Data.Projects;
    using ViewModels.Projects;

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

            inputModel.UploadInputModel.ProjectId = inputModel.Id;

            inputModel.Orders = inputModel.Orders.OrderByDescending(o => o.ModifiedOn).ToList();

            return this.View(inputModel);
        }
    }
}