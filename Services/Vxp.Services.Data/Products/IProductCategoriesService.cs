using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Products
{
    public interface IProductCategoriesService
    {
        bool IsCategoryExist(string categoryName);
        Task<TViewModel> CreateCategoryAsync<TViewModel>(TViewModel productCategory);
        IQueryable<TViewModel> GetAllCategories<TViewModel>();
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<bool> UpdateCategoryAsync(int categoryId, string newName);
    }
}