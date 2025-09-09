using Asp.Versioning.ApiExplorer;
using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Common.Logging.Correlations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Asp.Versioning;
using Catalog.API.SwaggerConfig;
using Common.Logging;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
// -----------------------------
// Base Path for Nginx or Ocelot
// -----------------------------
var nginxPath = "/catalog";

// -----------------------------
// Logging with Serilog
// -----------------------------
builder.Host.UseSerilog(Logging.ConfigureLogger);

// -----------------------------
// Add Services to the Container
// -----------------------------

// MVC Controller Support
builder.Services.AddControllers();

// API Versioning & Explorer (for Swagger)
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0); // Default to v1.0
        options.AssumeDefaultVersionWhenUnspecified = true; // Fallback to default if version not specified
        options.ReportApiVersions = true; // Report available versions
    })
    .AddApiExplorer(options =>
    {
        options.SubstituteApiVersionInUrl = true; // Replace {version} in route URL
    });

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Server definitions for Swagger UI
    c.AddServer(new OpenApiServer { Url = nginxPath, Description = "Catalog for nginx" });
    c.AddServer(new OpenApiServer { Url = "test", Description = "this is a test" });
});
builder.Services
    .AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigOptions>(); // Versioned Swagger options

// Logging & Correlation Middleware
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddHttpContextAccessor();

// AutoMapper Setup
builder.Services.AddAutoMapper(typeof(ProfileMapper));

// MediatR for CQRS
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(GetAllBrandsQueryHandler).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

// Repositories and Data Access
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ITypeRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, ProductRepository>();

// -----------------------------
// IdentityServer Authentication & Authorization
// -----------------------------
var authorizationPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Global authorization filter
builder.Services.AddControllers(config => { config.Filters.Add(new AuthorizeFilter(authorizationPolicy)); });

// Accept any server certificate (for dev or self-signed certs)
var httpHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};

//Configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // options.Authority = "http://identityserveraspnetidentity:8080"; // IdentityServer endpoint (Docker/Ocelot)
        // options.Authority = "https://id-local.eshopping.com:44344";   // For Nginx deployment (commented)
        options.Authority = "https://localhost:9009";

        options.Audience = "Catalog"; // API resource name
        options.RequireHttpsMetadata = false; // Disable HTTPS requirement
        options.BackchannelHttpHandler = httpHandler; // Accept all certs
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidIssuers =
            [
                "https://localhost:9009"
                // "http://identityserveraspnetidentity:8080", // Ocelot
                // "https://id-local.eshopping.com:44344"  // Nginx
            ],
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

// Custom authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanRead", policy => policy.RequireClaim("scope", "catalogapi.read"));
    options.AddPolicy("CanWrite", policy => policy.RequireClaim("scope", "catalogapi.write"));
});

// -----------------------------
// Build and Configure the Application
// -----------------------------
var app = builder.Build();

// Add correlation ID middleware (for tracing requests)
app.AddCorrelationIdMiddleware();

// Set application base path (important for reverse proxy like Nginx or Ocelot)
app.UsePathBase(nginxPath);

// Configure Forwarded Headers (for reverse proxy support)
var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto |
                       ForwardedHeaders.XForwardedHost
};
forwardedHeaderOptions.KnownNetworks.Clear(); // Clear default trusted networks
forwardedHeaderOptions.KnownProxies.Clear(); // Clear default trusted proxies
// forwardedHeaderOptions.KnownProxies.Add(IPAddress.Parse("172.18.0.16")); // Docker internal IP (if needed)
app.UseForwardedHeaders(forwardedHeaderOptions);

// -----------------------------
// Dev Environment Configuration
// -----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed error pages
    app.MapOpenApi(); // Register minimal OpenAPI endpoint

    // Enable Swagger and SwaggerUI
    app.UseSwagger(); // Generate Swagger JSON
    var versionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger"; // Route prefix can be empty or 'swagger'
        foreach (var desc in versionDescProvider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"{nginxPath}/swagger/{desc.GroupName}/swagger.json",
                $"Catalog.API - {desc.GroupName.ToUpper()}");
        }
    });
}

// -----------------------------
// Debug Middleware: Log Request Headers
// -----------------------------
app.Use(async (ctx, next) =>
{
    Console.WriteLine("---- BEGIN REQUEST HEADERS ----");
    foreach (var h in ctx.Request.Headers)
    {
        Console.WriteLine($"{h.Key}: {h.Value}");
    }

    Console.WriteLine("----  END REQUEST HEADERS  ----");
    await next();
});

// -----------------------------
// Final Middleware Pipeline
// -----------------------------
// app.UseAuthentication(); // Authenticate incoming requests
// app.UseAuthorization(); // Authorize requests based on policy
app.MapControllers(); // Map controller routes

// -----------------------------
// Run the Application
// -----------------------------
app.Run();