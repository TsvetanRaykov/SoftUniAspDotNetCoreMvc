﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vxp.Services.Data.Projects;
using Vxp.Web.ViewModels.Projects;

namespace Vxp.Web.Areas.Distributor.Controllers
{
    public class ProjectsController : DistributorsController
    {
        private readonly IProjectsService _projectsService;

        public ProjectsController(IProjectsService projectsService)
        {
            this._projectsService = projectsService;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await this._projectsService
                .GetAllProjects<ProjectViewModel>(this.User.Identity.Name)
                .ToListAsync();

            return this.View(projects);
        }
    }
}