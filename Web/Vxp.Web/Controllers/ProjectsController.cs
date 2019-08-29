using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Vxp.Services.Models;
using Vxp.Web.ViewModels.Documents;

namespace Vxp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Projects;
    using Vxp.Services.Data.Projects;
    using System.Threading.Tasks;
    using Infrastructure.Attributes.ActionFilters;
    using System.Security.Claims;

    [Authorize]
    [ValidateAntiForgeryToken]
    public class ProjectsController : BaseController
    {
        private readonly IProjectsService _projectsService;
        private readonly IFilesService _filesService;
        public ProjectsController(IProjectsService projectsService, IFilesService filesService)
        {
            this._projectsService = projectsService;
            this._filesService = filesService;
        }

        [HttpPost]
        [SetTempDataModelState]
        public async Task<IActionResult> Create([FromForm(Name = "Input")] ProjectInputModel project)
        {
            this.ModelState.Remove("Input.UploadInputModel.Description");
            this.ModelState.Remove("Input.UploadInputModel.FormFile");
            if (this.ModelState.IsValid)
            {
                project.OwnerId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                await this._projectsService.CreateProjectAsync(project);
            }

            return this.Redirect(project.ReturnUrl);
        }

        [HttpPost]
        [SetTempDataModelState]
        public async Task<IActionResult> Update(ProjectInputModel project)
        {

            if (this.ModelState.IsValid)
            {
                if (project.OwnerId != this.User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return this.Redirect("/Identity/Account/AccessDenied");
                }

                await this._projectsService.UpdateProjectAsync(project);
            }

            return this.Redirect(project.ReturnUrl);
        }

        [HttpPost]
        [SetTempDataModelState]
        public async Task<IActionResult> Delete([FromForm(Name = "project.Id")]int projectId, [FromForm(Name = "project.ReturnUrl")]string returnUrl)
        {
            var project = await this._projectsService.GetAllProjects<ProjectDto>(this.User.Identity.Name)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return this.NotFound();
            }

            if (project.OwnerId != this.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return this.Redirect("/Identity/Account/AccessDenied");
            }

            await this._projectsService.DeleteProjectAsync(project.Id);

            return this.Redirect(returnUrl);
        }

        [HttpPost]
        [SetTempDataModelState]
        [RequestSizeLimit(5242880)]
        public async Task<IActionResult> UploadProjectDocument(DocumentUploadInputModel inputModel)
        {
            if (this.ModelState.IsValid)
            {
                if (!this._filesService.ValidateFileType(inputModel.FormFile.ContentType))
                {
                    this.ModelState.AddModelError("UploadInputModel", "The file type you're trying to upload is no allowed!");
                    return this.Redirect(inputModel.ReturnUrl);
                }

                var storeFile = AutoMapper.Mapper.Map<FileStoreDto>(inputModel);
                storeFile.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var newFile = await this._filesService.StoreFileToDatabaseAsync(storeFile);
                if (!string.IsNullOrWhiteSpace(newFile.Location))
                {
                    try
                    {
                        using (var fileStream = new FileStream(newFile.Location, FileMode.Create, FileAccess.Write))
                        {
                            await inputModel.FormFile.CopyToAsync(fileStream);
                        }
                    }
                    catch
                    {
                        await this._filesService.DeleteFileFromDataBaseAsync(newFile.Id);
                    }

                }

            }
            return this.Redirect(inputModel.ReturnUrl);
        }

        [HttpPost]
        [SetTempDataModelState]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var file = await this._filesService.GetDocumentByIdAsync(id);
            if (file != null)
            {
                var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!new[] { file.Project.OwnerId, file.Project.PartnerId }.Contains(currentUserId))
                {
                    return this.Redirect("/Identity/Account/AccessDenied");
                }

                if (!System.IO.File.Exists(file.Location))
                {
                    return this.NotFound();
                }

                return this.File(new FileStream(file.Location, FileMode.Open, FileAccess.Read), file.ContentType,
                    file.OriginalFileName);
            }

            return this.NotFound();
        }
    }
}