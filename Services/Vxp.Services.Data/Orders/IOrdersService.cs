namespace Vxp.Services.Data.Orders
{
    public interface IOrdersService
    {
        bool GetAllOrderedProducts(int orderId);
    }
}