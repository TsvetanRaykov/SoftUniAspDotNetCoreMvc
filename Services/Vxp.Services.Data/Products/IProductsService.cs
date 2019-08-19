using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Products
{
    public interface IProductsService
    {
        Task<bool> IsProductExist<TViewModel>(TViewModel product);
        Task<TViewModel> CreateProductAsync<TViewModel>(TViewModel product);
        IQueryable<TViewModel> GetAllProducts<TViewModel>();
        Task<bool> DeleteProductAsync(int productId);
        Task<TViewModel> UpdateProductAsync<TViewModel>(TViewModel product);
        Task<List<TViewModel>> GetDeletedProducts<TViewModel>();
        Task<bool> DeletePermanentlyAsync(int id);
        Task<bool> RestoreAsync(int id);
    }
}