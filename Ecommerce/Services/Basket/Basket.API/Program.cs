using System.Reflection;
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
using ServiceDefaults;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
// -----------------------------
// Serilog Logging
// -----------------------------
builder.Host.UseSerilog(Logging.ConfigureLogger);

// -----------------------------
// Add Services to the Container
// -----------------------------

// MVC Controller Support
builder.Services.AddControllers();

// API Versioning & API Explorer (for Swagger grouping)
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0); // Set default API version to 1.0
        options.AssumeDefaultVersionWhenUnspecified = true; // Use default version when none is specified
        options.ReportApiVersions = true; // Add API versions to response headers
        //#1
        // options.ApiVersionReader = new HeaderApiVersionReader("X-Version"); // Read version from custom header "X-Version"
        // options.ApiVersionReader = new MediaTypeApiVersionReader("X-Version"); // Read version from media type (e.g., application/vnd.eshopping.v1+json)
        // options.ApiVersionReader =
        //     new QueryStringApiVersionReader("X-Version"); // Read version from query string parameter "X-Version"
        //#2
        // options.ApiVersionReader = ApiVersionReader.Combine(
        //     new HeaderApiVersionReader("H-Version"),
        //     new MediaTypeApiVersionReader("M-Version"),
        //     new QueryStringApiVersionReader("Q-Version"));
    })
    .AddApiExplorer(options =>
    {
        options.SubstituteApiVersionInUrl = true; // Enable version substitution in route URLs
    });

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
builder.Services.AddSwaggerGen(); // Add Swagger generator
builder.Services
    .AddTransient<IConfigureOptions<SwaggerGenOptions>,
        SwaggerConfigOptions>(); // Custom Swagger options based on API versioning

// Logging & Correlation
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddHttpContextAccessor();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(BasketMappingProfile));

// MediatR (CQRS)
var assemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(CreateShoppingCartCommandHandler).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Dependency Injection for Repositories
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// gRPC Client for Discount Service
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});

// MassTransit (RabbitMQ)
builder.Services.AddMassTransit(configuration =>
{
    configuration.UsingRabbitMq((_, cfg) => { cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]); });
});
builder.Services.AddMassTransitHostedService();

// Authentication & Authorization with IdentityServer
var authorizationPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddControllers(config =>
{
    config.Filters.Add(new AuthorizeFilter(authorizationPolicy)); // Apply global authorization policy
});

// Configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // IdentityServer URL برای Docker
        options.Authority = "https://host.docker.internal:9009";
        options.Audience = "Basket";
        options.RequireHttpsMetadata = false;
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            // ValidAudiences =
            // [
            //     "Catalog", "EShoppingGateway", "Basket", "eshoppingAngular"
            // ],
            ValidAudiences = ["Basket"],
            ValidIssuers = ["https://host.docker.internal:9009"]
        };

        options.IncludeErrorDetails = true;

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Auth failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully.");
                return Task.CompletedTask;
            }
        };
    });

// -----------------------------
// Build the Application
// -----------------------------
var app = builder.Build();

// Correlation ID Middleware (must be before endpoints)
app.AddCorrelationIdMiddleware();

// Reverse Proxy (e.g., Nginx) Support
var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// forwardedHeaderOptions.KnownNetworks.Clear(); // Uncomment if necessary
// forwardedHeaderOptions.KnownProxies.Clear();  // Uncomment if necessary
app.UseForwardedHeaders(forwardedHeaderOptions);

// -----------------------------
// Development-only Middleware
// -----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detailed error pages in development
    app.MapOpenApi(); // Minimal API OpenAPI mapping

    // Swagger UI Configuration
    app.UseSwagger(); // Generate Swagger JSON
    app.UseSwaggerUI(c =>
    {
        var versionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var desc in versionDescProvider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"{desc.GroupName}/swagger.json", $"Basket.API - {desc.GroupName.ToUpper()}");
        }
    });
}

// -----------------------------
// Request Pipeline
// -----------------------------
// app.UseAuthentication(); // Authenticate the request
// app.UseAuthorization(); // Authorize the request
app.MapControllers(); // Map controller endpoints

// -----------------------------
// Run the Application
// -----------------------------
app.Run();