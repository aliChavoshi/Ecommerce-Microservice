using Asp.Versioning;
using Common.Logging;
using Common.Logging.Correlations;
using EventBus.Messages.Common;
using MassTransit;
using Microsoft.OpenApi.Models;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//Add Service for Serilog
builder.Host.UseSerilog(Logging.ConfigureLogger);

//Logging & Correlation
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

builder.Services.AddControllers();
// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1", Description = "Ordering API" });
});
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
//Add RabbitMQ : Consumer
builder.Services.AddScoped<BasketOrderingConsumer>();
builder.Services.AddScoped<BasketOrderingConsumerV2>();
builder.Services.AddMassTransit(configuration =>
{
    // Add all consumers from the current assembly
    // configuration.AddConsumer<BasketOrderingConsumer>();
    // configuration.AddConsumer<BasketOrderingConsumerV2>();
    configuration.AddConsumers(Assembly.GetExecutingAssembly());
    configuration.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        // Provide the QueueName with consumer settings
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue,
            e => e.ConfigureConsumer<BasketOrderingConsumer>(ctx));
        // V2
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueueV2,
            e => e.ConfigureConsumer<BasketOrderingConsumerV2>(ctx));
    });
});
builder.Services.AddMassTransitHostedService();
var app = builder.Build();
//Correlation and Logging
app.AddCorrelationIdMiddleware(); // معمولاً بعد از app.UseRouting و قبل از UseEndpoints

app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger!).Wait();
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();