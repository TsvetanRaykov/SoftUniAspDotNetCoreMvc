namespace Vxp.Services.Data.Projects
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;
    using Mapping;

    public class ProjectsService : IProjectsService
    {
        private readonly IDeletableEntityRepository<Project> _projectsRepository;

        public ProjectsService(IDeletableEntityRepository<Project> projectsRepository)
        {
            this._projectsRepository = projectsRepository;
        }

        public async Task<int> CreateProjectAsync<TViewModel>(TViewModel project)
        {
            var newProject = AutoMapper.Mapper.Map<Project>(project);
            await this._projectsRepository.AddAsync(newProject);
            await this._projectsRepository.SaveChangesAsync();
            return newProject.Id;
        }

        public async Task<TViewModel> UpdateProjectAsync<TViewModel>(TViewModel project)
        {
            var projectUpdate = AutoMapper.Mapper.Map<Project>(project);
            var projectFromDb = await this._projectsRepository.AllWithDeleted()
                .FirstOrDefaultAsync(p => p.Id == projectUpdate.Id);

            if (projectFromDb == null)
            {
                return default;
            }

            AutoMapper.Mapper.Map(project, projectFromDb);
            this._projectsRepository.Update(projectFromDb);
            await this._projectsRepository.SaveChangesAsync();
            return AutoMapper.Mapper.Map<TViewModel>(projectFromDb);
        }

        public IQueryable<TViewModel> GetAllProjects<TViewModel>(string userName)
        {
            var projectsFromDb = this._projectsRepository.AllAsNoTracking()
                .Where(p => p.Owner.UserName == userName);
            
            return projectsFromDb.To<TViewModel>();
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            var projectsFromDb = await this._projectsRepository.GetByIdWithDeletedAsync(projectId);
            if (projectsFromDb == null)
            {
                return false;
            }
            this._projectsRepository.Delete(projectsFromDb);
            await this._projectsRepository.SaveChangesAsync();
            return true;
        }
    }
}