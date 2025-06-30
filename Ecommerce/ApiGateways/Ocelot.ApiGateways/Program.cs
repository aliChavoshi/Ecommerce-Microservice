using Common.Logging;
using Common.Logging.Correlations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.ApiGateways.Handlers;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Logging.ConfigureLogger);

//ocelot configuration
builder.Host.ConfigureAppConfiguration((env, config) =>
{
    var environmentName = env.HostingEnvironment?.EnvironmentName ?? "Development";
    if (config != null) config.AddJsonFile($"ocelot.{environmentName}.json", true, true);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Logging & Correlation : TODO important
builder.Services.AddTransient<ICorrelationIdGenerator, CorrelationIdGenerator>();

//CORS
var corsPolicyName = "AllowAllOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy
                // .AllowAnyOrigin()
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddTransient<CorrelationDelegatingHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddOcelot()
    .AddDelegatingHandler<CorrelationDelegatingHandler>() // Add the correlation handler
    .AddCacheManager(x => x.WithDictionaryHandle());

var authScheme = "EShoppingGatewayAuthScheme";
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//CORS
app.UseCors(corsPolicyName);
//Correlations and Logging
app.AddCorrelationIdMiddleware();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello Ocelot");
//FOR Test
app.Use(async (context, next) =>
{
    var correlation = context.RequestServices.GetRequiredService<ICorrelationIdGenerator>();
    var correlationId = correlation.Get();
    Log.Information("Middleware CorrelationId In Ocelot: {correlationId}", correlationId);

    await next();
});

await app.UseOcelot();
await app.RunAsync();