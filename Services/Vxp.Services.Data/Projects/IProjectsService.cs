using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Projects
{
    public interface IProjectsService
    {
        Task<int> CreateProjectAsync<TViewModel>(TViewModel project);
        Task<TViewModel> GetProjectAsync<TViewModel>(int projectId);
        IQueryable<TViewModel> GetAllProjects<TViewModel>(string userName);
    }
}