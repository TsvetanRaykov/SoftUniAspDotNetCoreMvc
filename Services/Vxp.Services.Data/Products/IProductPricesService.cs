namespace Vxp.Services.Data.Products
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface IProductPricesService
    {
        IQueryable<TViewModel> GetBuyerPriceModifiers<TViewModel>(string userName);
        IQueryable<TViewModel> GetSellerPriceModifiers<TViewModel>(string userName);
        Task<bool> DeleteProductPriceModifierAsync(int priceModifier);
        Task SetProductPriceModifierAsync<TViewModel>(TViewModel priceModifier);
    }
}