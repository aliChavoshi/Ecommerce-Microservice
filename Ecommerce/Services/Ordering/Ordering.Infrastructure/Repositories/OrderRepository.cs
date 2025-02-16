using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository(OrderContext context) : GenericRepository<Order>(context), IOrderRepository
{
    private readonly OrderContext _context = context;

    public async Task<IEnumerable<Order>> GetOrdersByUserName(string? userName)
    {
        return await _context.Orders.Where(x => x.UserName == userName).ToListAsync();
    }
}