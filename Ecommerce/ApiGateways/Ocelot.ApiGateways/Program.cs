﻿using Common.Logging;
using Common.Logging.Correlations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

//Logging & Correlation
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

var authScheme = "EShoppingGatewayAuthScheme";
builder.Services
    .AddOcelot()
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Correlations and Logging
app.AddCorrelationIdMiddleware();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello Ocelot");

await app.UseOcelot();
await app.RunAsync();

