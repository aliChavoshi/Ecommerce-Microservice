using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Ordering.API.Extensions;

public static class DbExtension
{
    public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetService<TContext>();

        try
        {
            logger.LogInformation($"Started Db Migration: {typeof(TContext).Name}");
            //retry strategy
            var retry = Policy.Handle<SqlException>()
                .WaitAndRetry(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, span, count) => { logger.LogError($"Retrying because of {exception} {span}"); });
            retry.Execute(async () => await CallSeeder(seeder, context, services));
            logger.LogInformation($"Migration Completed: {typeof(TContext).Name}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An Error occurred while migrating db: {typeof(TContext).Name}");
        }

        return host;
    }

    private static async Task CallSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext? context,
        IServiceProvider services) where TContext : DbContext
    {
        if (context != null)
        {
            await context.Database.MigrateAsync();
            seeder(context, services); //setting of default values on empty db 
        }
    }
}