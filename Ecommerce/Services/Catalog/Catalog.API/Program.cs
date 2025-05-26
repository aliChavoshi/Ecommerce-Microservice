using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Catalog.API.SwaggerConfig;
using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Common.Logging;
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
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
// Register Mapper
builder.Services.AddAutoMapper(typeof(ProfileMapper));
//Register Mediator
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(GetAllBrandsQueryHandler).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

// Register Repositories
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ITypeRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, ProductRepository>();

//IdentityServer
var authorizationPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
builder.Services.AddControllers(config => { config.Filters.Add(new AuthorizeFilter(authorizationPolicy)); });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://identityserveraspnetidentity:8080"; // نکته مهم
        // options.Authority = "https://id-local.eshopping.com:44344"; // نکته مهم
        options.Audience = "Catalog";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "http://identityserveraspnetidentity:8080",
            // ValidIssuer = "https://id-local.eshopping.com:44344",
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanRead", policy => policy.RequireClaim("scope", "catalogapi.read"));
    options.AddPolicy("CanWrite", policy => policy.RequireClaim("scope", "catalogapi.write"));
});
//End Identity
var app = builder.Build();
var versionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
//for Nginx reverse proxy
var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// forwardedHeaderOptions.KnownNetworks.Clear();
// forwardedHeaderOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeaderOptions);
var nginxPath = "/catalog";
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
            c.SwaggerEndpoint($"{desc.GroupName}/swagger.json",
                $"Catalog.API - {desc.GroupName.ToUpper()}");
        }
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();