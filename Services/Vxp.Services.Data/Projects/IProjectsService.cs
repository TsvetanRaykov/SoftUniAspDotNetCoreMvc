using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Projects
{
    public interface IProjectsService
    {
        Task<int> CreateProjectAsync<TViewModel>(TViewModel project);
        Task<TViewModel> UpdateProjectAsync<TViewModel>(TViewModel project);
        IQueryable<TViewModel> GetAllProjects<TViewModel>(string userName);
        Task<bool> DeleteProjectAsync(int projectId);
    }
}