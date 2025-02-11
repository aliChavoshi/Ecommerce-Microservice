using Ordering.Core.Entities;

namespace Ordering.Core.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
}