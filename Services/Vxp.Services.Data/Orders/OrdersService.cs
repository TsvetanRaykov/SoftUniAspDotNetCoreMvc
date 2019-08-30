using System;

namespace Vxp.Services.Data.Orders
{
    using Microsoft.EntityFrameworkCore;
    using Web.Infrastructure.Extensions;
    using Mapping;
    using System.Linq;
    using System.Threading.Tasks;
    using Vxp.Data.Common.Enums;
    using Vxp.Data.Common.Repositories;
    using Vxp.Data.Models;

    public class OrdersService : IOrdersService
    {
        private readonly IDeletableEntityRepository<Order> _ordersRepository;

        public OrdersService(IDeletableEntityRepository<Order> ordersRepository)
        {
            this._ordersRepository = ordersRepository;
        }

        public Task<IQueryable<TViewModel>> GetAllOrdersAsync<TViewModel>(string userName)
        {
            return Task.Run(() =>
            {
                return this._ordersRepository.AllAsNoTrackingWithDeleted().Where(o => o.Buyer.UserName == userName)
                    .To<TViewModel>();
            });
        }

        public async Task<bool> CreateOrderAsync<TViewModel>(TViewModel order, string buyerId)
        {
            var newOrder = AutoMapper.Mapper.Map<Order>(order);
            newOrder.BuyerId = buyerId;

            await this._ordersRepository.AddAsync(newOrder);
            await this._ordersRepository.SaveChangesAsync();

            if (newOrder.Id > 0)
            {
                newOrder = await this._ordersRepository.All()
                    .Include(o => o.Products)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(p => p.Details)
                    .ThenInclude(p => p.CommonDetail)
                    .Include(p => p.Products)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(o => o.Id == newOrder.Id);

                foreach (var product in newOrder.Products)
                {
                    product.ProductData = product.Product.ToJson();
                }

                await this.UpdateOrderStatusAsync(newOrder.Id, OrderStatus.New);
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var orderFromDb = await this._ordersRepository.AllWithDeleted()
                .Include(o => o.Project)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (orderFromDb == null)
            {
                return false;
            }

            var newHistoryRecord = new OrderHistory
            {
                OldStatus = orderFromDb.Status,
                NewStatus = newStatus
            };

            orderFromDb.Project.ModifiedOn = DateTime.UtcNow;

            orderFromDb.OrderHistories.Add(newHistoryRecord);
            orderFromDb.Status = newStatus;
            this._ordersRepository.Update(orderFromDb);
            await this._ordersRepository.SaveChangesAsync();

            return true;
        }
    }
}