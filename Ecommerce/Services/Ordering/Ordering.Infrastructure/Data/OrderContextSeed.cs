using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
    {
        if (!await orderContext.Orders.AnyAsync())
        {
            orderContext.Orders.AddRange(GetPreconfiguredOrders());
            await orderContext.SaveChangesAsync();
            logger.LogInformation("Seed database associated with context {DbContextName}", nameof(OrderContext));
        }
    }

    private static List<Order> GetPreconfiguredOrders()
    {
        return
        [
            new Order
            {
                UserName = "rahul",
                FirstName = "Rahul",
                LastName = "Sahay",
                EmailAddress = "rahulsahay@eCommerce.net",
                AddressLine = "Bangalore",
                TotalPrice = 750,
                State = "KA",
                CreatedBy = "Rahul",
                PaymentMethod = 1,
                LastModifiedBy = "Rahul",
                LastModifiedDate = new DateTime(),
            }
        ];
    }
}