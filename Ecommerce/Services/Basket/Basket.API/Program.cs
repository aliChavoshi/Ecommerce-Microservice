﻿using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Basket.API.SwaggerConfig;
using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mappers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Services;
using Common.Logging;
using Discount.Application.Protos;
using MassTransit;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
//Add Service for Serilog
builder.Host.UseSerilog(Logging.ConfigureLogger);
// Add services to the container.
builder.Services.AddControllers();
// Add API Versioning and API Explorer for Swagger
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigOptions>();
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
    configuration.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});
builder.Services.AddMassTransitHostedService();
//Build
var app = builder.Build();
var versionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Configure the HTTP request pipeline.
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
            c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"Basket.API - {desc.GroupName.ToUpper()}");
        }
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();