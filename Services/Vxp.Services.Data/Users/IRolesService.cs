using System.Linq;

namespace Vxp.Services.Data.Users
{
    public interface IRolesService
    {
        IQueryable<TViewModel> GetAll<TViewModel>();
        int Count();
    }
}