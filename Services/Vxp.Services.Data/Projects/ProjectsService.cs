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

        public IQueryable<TViewModel> GetAllProjects<TViewModel>(string userName)
        {
            var projectsFromDb = this._projectsRepository.AllAsNoTrackingWithDeleted()
                .Where(p => p.Owner.UserName == userName);

            return projectsFromDb.To<TViewModel>();
        }

        public async Task<TViewModel> GetProjectAsync<TViewModel>(int projectId)
        {
            var projectFromDb = await this._projectsRepository.AllAsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId);

            return AutoMapper.Mapper.Map<TViewModel>(projectFromDb);
        }
    }
}