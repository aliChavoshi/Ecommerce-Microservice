using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Basket.API.SwaggerConfig;
using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mappers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Services;
using Common.Logging;
using Common.Logging.Correlations;
using Discount.Application.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//Add Service for Serilog
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
builder.Services.AddControllers();
// Add API Version and API Explorer for Swagger
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigOptions>();

//Logging & Correlation
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.SubstituteApiVersionInUrl = true; // this is for set default version in url
    });
//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register Mapper
builder.Services.AddAutoMapper(typeof(BasketMappingProfile));
//Register Mediatr
var assemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(CreateShoppingCartCommandHandler).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
//Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
//DI
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
//GRPC
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});
//Add RabbitMQ
builder.Services.AddMassTransit(configuration =>
{
    configuration.UsingRabbitMq((_, cfg) => { cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]); });
});
builder.Services.AddMassTransitHostedService();

//Identity Server
var authorizationPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
builder.Services.AddControllers(config => { config.Filters.Add(new AuthorizeFilter(authorizationPolicy)); });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://identityserveraspnetidentity:8080"; // for ocelot
        // options.Authority = "https://id-local.eshopping.com:44344"; // for nginx
        options.Audience = "Basket";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidIssuer = "http://identityserveraspnetidentity:8080",
            // ValidIssuer = "https://id-local.eshopping.com:44344",
        };
    });

//Build
var app = builder.Build();
//Correlation and Logging
app.AddCorrelationIdMiddleware(); // معمولاً بعد از app.UseRouting و قبل از UseEndpoints

var versionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
//for Nginx reverse proxy
var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// forwardedHeaderOptions.KnownNetworks.Clear();
// forwardedHeaderOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeaderOptions);
// Configure the HTTP request pipeline.
// var nginxPath = "/basket";
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    //Swagger
    app.UseSwagger(); // generate the json file for swagger
    // for presentation for dropdown
    app.UseSwaggerUI(c =>
    {
        foreach (var desc in versionDescProvider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"{desc.GroupName}/swagger.json", $"Basket.API - {desc.GroupName.ToUpper()}");
        }
    });
}

app.UseAuthentication(); // Identity Server
app.UseAuthorization();
app.MapControllers();
app.Run();