namespace Vxp.Services.Data.Orders
{
    using Vxp.Data.Common.Enums;
    using System.Threading.Tasks;
    using System.Linq;

    public interface IOrdersService
    {
        Task<IQueryable<TViewModel>> GetAllOrdersAsync<TViewModel>(string userName);
        Task<bool> CreateOrderAsync<TViewModel>(TViewModel order, string buyerId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}