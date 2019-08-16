using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Products
{
    public interface IProductsService
    {
        bool IsProductExist(string productName, string categoryName);
        Task<TViewModel> CreateProductAsync<TViewModel>(TViewModel product);
        IQueryable<TViewModel> GetAllProducts<TViewModel>();
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> UpdateProductAsync<TViewModel>(TViewModel product);
    }
}