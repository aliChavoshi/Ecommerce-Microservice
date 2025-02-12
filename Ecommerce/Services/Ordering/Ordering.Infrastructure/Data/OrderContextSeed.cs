using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
    {
        if (!orderContext.Orders.Any())
        {
            orderContext.Orders.AddRange(GetPreconfiguredOrders());
            await orderContext.SaveChangesAsync();
            logger.LogInformation("Seed database associated with context {DbContextName}", nameof(OrderContext));
        }
    }

    private static Order[] GetPreconfiguredOrders()
    {
        return
        [
            new Order
            {
                UserName = "Ali Chavoshi",
                FirstName = "Ali",
                LastName = "Chavoshi",
                EmailAddress = "alichavoshi@eCommerce.net",
                AddressLine = "Kashan",
                TotalPrice = 750,
                State = "Kashan",
                CreatedBy = "Ali Chavoshi",
                PaymentMethod = 1,
                LastModifiedBy = "Ali Chavoshi",
                LastModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now
            }
        ];
    }
}