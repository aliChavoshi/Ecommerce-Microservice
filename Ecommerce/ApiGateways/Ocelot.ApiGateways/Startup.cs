using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;

namespace Ocelot.ApiGateways;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOcelot().AddCacheManager(o => o.WithDictionaryHandle());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello Ocelot.");
            });
        });
    }
}