using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Infrastructure.Extensions;

public static class DbExtension
{
    public static IHost MigrateDatabase<TContext>(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}",
                typeof(TContext).Name);
            ApplyMigration(configuration, logger);
            logger.LogInformation("Migrated database associated with context {DbContextName}",
                typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}",
                typeof(TContext).Name);
        }

        return host;
    }

    private static void ApplyMigration(IConfiguration config, ILogger logger)
    {
        var retry = 5;
        while (retry > 0)
        {
            try
            {
                logger.LogInformation("ApplyMigration was started");
                using var connection =
                    new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));
                connection.Open();
                using var cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "DROP TABLE IF EXISTS Coupon";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                    ProductName VARCHAR(500) NOT NULL,
                                                    Description TEXT,
                                                    Amount INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Adidas Quick Force Indoor Badminton Shoes', 'Shoe Discount', 500);";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Yonex VCORE Pro 100 A Tennis Racquet (270gm, Strung)', 'Racquet Discount', 700);";
                cmd.ExecuteNonQuery();
                // Exit loop if successful
                break;
            }
            catch (Exception ex)
            {
                retry--;
                if (retry == 0)
                {
                    throw;
                }

                //Wait for 2 seconds
                Thread.Sleep(2000);
            }
        }
    }
}