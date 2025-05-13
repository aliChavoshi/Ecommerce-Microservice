using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.ApiGateways.Handlers;

var builder = WebApplication.CreateBuilder(args);

//ocelot configuration
builder.Host.ConfigureAppConfiguration((env, config) =>
{
    var environmentName = env.HostingEnvironment?.EnvironmentName ?? "Development";
    if (config != null) config.AddJsonFile($"ocelot.{environmentName}.json", true, true);
});

var authScheme = "EShoppingGatewayAuthScheme";
builder.Services
    .AddOcelot()
    .AddDelegatingHandler<ForwardTokenHandler>()
    .AddCacheManager(x => x.WithDictionaryHandle());
//Identity Server
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(authScheme, options =>
    {
        options.Authority = "http://identityserveraspnetidentity:8080"; // نکته مهم
        options.Audience = "EShoppingGateway";
        options.RequireHttpsMetadata = false;
    });

//End Identity
var app = builder.Build();

if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
app.UseRouting();
app.MapGet("/", async context => { await context.Response.WriteAsync("Hello Ocelot"); });
await app.UseOcelot();