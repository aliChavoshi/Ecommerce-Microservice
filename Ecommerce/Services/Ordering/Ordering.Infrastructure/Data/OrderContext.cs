using Microsoft.EntityFrameworkCore;
using Ordering.Core.Common;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data;

public class OrderContext(DbContextOptions<OrderContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        //we can use interceptors instead of this code
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.LastModifiedDate = DateTime.Now;
                    entry.Entity.LastModifiedBy = "Ali Chavoshi"; //TODO
                    break;
                case EntityState.Added:
                    entry.Entity.LastModifiedDate = DateTime.Now;
                    entry.Entity.LastModifiedBy = "Ali Chavoshi"; //TODO
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.CreatedBy = "Ali Chavoshi"; //TODO
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
};