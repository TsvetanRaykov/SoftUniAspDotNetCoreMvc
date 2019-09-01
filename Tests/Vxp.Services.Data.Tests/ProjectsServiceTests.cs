using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vxp.Data.Models;
using Vxp.Data.Repositories;
using Vxp.Services.Data.Projects;
using Vxp.Services.Models;
using Xunit;

namespace Vxp.Services.Data.Tests
{
    [Collection("Database collection")]
    public class ProjectsServiceTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly IProjectsService _projectsService;

        public ProjectsServiceTests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
            this._projectsService = GetProjectsService();
        }

        [Fact]
        public async Task GetAllProjectsShouldReturnAllProjectsOfUser()
        {
            Mapping.Config(typeof(ProjectDto));
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var projectsRepository = new EfDeletableEntityRepository<Project>(this._fixture.DbContext);

            var user = await usersRepository.AllWithDeleted().FirstOrDefaultAsync();

            for (int i = 0; i < 10; i++)
            {
                await projectsRepository.AddAsync(new Project
                {
                    Name = $"TestProject{i}",
                    OwnerId = user.Id,
                    PartnerId = user.Id
                });
            }

            await projectsRepository.SaveChangesAsync();

            var projects = await this._projectsService.GetAllProjects<ProjectDto>(user.UserName).ToArrayAsync();

            Assert.Equal(10, projects.Length);

        }

        [Fact]
        public async Task DeleteProjectsTest()
        {
            Mapping.Config(typeof(ProjectDto));
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var projectsRepository = new EfDeletableEntityRepository<Project>(this._fixture.DbContext);

            var user = await usersRepository.AllWithDeleted().FirstOrDefaultAsync();

            for (int i = 0; i < 10; i++)
            {
                await projectsRepository.AddAsync(new Project
                {
                    Name = $"TestProject{i}",
                    OwnerId = user.Id,
                    PartnerId = user.Id
                });
            }
            await projectsRepository.SaveChangesAsync();

            var projectsToDelete = projectsRepository.AllAsNoTrackingWithDeleted().Where(p => p.OwnerId == user.Id).ToArray();

            foreach (var project in projectsToDelete)
            {
                await this._projectsService.DeleteProjectAsync(project.Id);
            }

            var projects = await this._projectsService.GetAllProjects<ProjectDto>(user.UserName).ToArrayAsync();

            Assert.Empty(projects);

        }

        [Fact]
        public async Task DeleteProjectShouldReturnFalseIfNotFound()
        {
            var result = await this._projectsService.DeleteProjectAsync(int.MaxValue);
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateProjectTest()
        {
            Mapping.Config(typeof(ProjectDto));
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(this._fixture.DbContext);
            var projectsRepository = new EfDeletableEntityRepository<Project>(this._fixture.DbContext);

            var user = await usersRepository.AllWithDeleted().FirstOrDefaultAsync();

            var newProject = new Project
            {
                Name = $"TestProject",
                OwnerId = user.Id,
                PartnerId = user.Id
            };
            await projectsRepository.AddAsync(newProject);

            await projectsRepository.SaveChangesAsync();

            var projectsToUpdate = await this._projectsService.GetAllProjects<ProjectDto>(user.UserName)
                .FirstOrDefaultAsync();

            var newName = Guid.NewGuid().ToString();
            projectsToUpdate.Name = newName;

            await this._projectsService.UpdateProjectAsync(projectsToUpdate);
            var result = await this._projectsService.GetAllProjects<ProjectDto>(user.UserName)
                .FirstOrDefaultAsync();

            Assert.Equal(newName, result.Name);
        }

        [Fact]
        public async Task UpdateProjectShouldReturnNullIfNotFound()
        {
            var projectModel = new ProjectDto
            {
                Id = int.MaxValue
            };
            Mapping.Config(typeof(ProjectDto));
            var result = await this._projectsService.UpdateProjectAsync(projectModel);

            Assert.Null(result);
        }

        private IProjectsService GetProjectsService()
        {
            var projectsRepository = new EfDeletableEntityRepository<Project>(this._fixture.DbContext);
            return new ProjectsService(projectsRepository);
        }
    }
}