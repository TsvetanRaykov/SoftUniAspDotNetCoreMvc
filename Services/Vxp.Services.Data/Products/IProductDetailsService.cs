using System.Linq;
using System.Threading.Tasks;

namespace Vxp.Services.Data.Products
{
    public interface IProductDetailsService
    {
        bool IsCommonProductDetailExist(string productDetailName, string measure);
        Task<TViewModel> CreateCommonProductDetailAsync<TViewModel>(TViewModel commonProductDetail);
        IQueryable<TViewModel> GetAllCommonProductDetails<TViewModel>();
        Task<bool> DeleteCommonProductDetailAsync(int productDetailId);
        Task<bool> UpdateCommonProductDetailAsync<TViewModel>(TViewModel commonProductDetail);
    }
}
