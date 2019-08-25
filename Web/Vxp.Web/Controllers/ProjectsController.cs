using System.IO;
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
                if (!this.ValidateUserRightsOn(project))
                {
                    return this.Redirect("/Identity/Account/AccessDenied");
                }

                await this._projectsService.UpdateProjectAsync(project);
            }

            return this.Redirect(project.ReturnUrl);
        }

        [HttpPost]
        [SetTempDataModelState]
        public async Task<IActionResult> Delete(ProjectInputModel project)
        {
            if (this.ModelState.IsValid)
            {
                if (!this.ValidateUserRightsOn(project))
                {
                    return this.Redirect("/Identity/Account/AccessDenied");
                }
                await this._projectsService.DeleteProjectAsync(project.Id);
            }

            return this.Redirect(project.ReturnUrl);
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
                await this._filesService.StoreFileAsync(storeFile);
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
                if (file.UserId != this.User.FindFirstValue(ClaimTypes.NameIdentifier))
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


        private bool ValidateUserRightsOn(ProjectInputModel project)
        {
            return project.OwnerId == this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}