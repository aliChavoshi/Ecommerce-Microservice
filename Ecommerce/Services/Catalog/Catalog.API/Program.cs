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
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// مسیر پایه برای Nginx و Ocelot
var nginxPath = "/catalog";

// Add Service for Serilog
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
builder.Services.AddSwaggerGen(c =>
{
    // این خط را برای اضافه کردن سرور به OpenApi spec حفظ می‌کنیم.
    // اگرچه در سناریوهای پیچیده‌تر ممکن است نیاز به تنظیمات دقیق‌تر یا حذف داشته باشد،
    // برای سادگی و حفظ کامنت شما، آن را نگه می‌داریم.
    c.AddServer(new OpenApiServer { Url = nginxPath });
});

// builder.Services.AddOpenApi(); // .

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
        // options.Authority = "http://identityserveraspnetidentity:8080"; // نکته مهم (کامنت شما)
        options.Authority = "https://id-local.eshopping.com:44344";

        options.Audience = "Catalog";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidIssuers =
            [
                // "http://identityserveraspnetidentity:8080", //ocelot
                "https://id-local.eshopping.com:44344" //nginx
            ],
        };
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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanRead", policy => policy.RequireClaim("scope", "catalogapi.read"));
    options.AddPolicy("CanWrite", policy => policy.RequireClaim("scope", "catalogapi.write"));
});
//End Identity

var app = builder.Build();

// ۱) این خط را اضافه کن (PathBase را برای برنامه تنظیم می‌کند)
app.UsePathBase(nginxPath);

//for Nginx reverse proxy
var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// forwardedHeaderOptions.KnownNetworks.Clear(); // .
// forwardedHeaderOptions.KnownProxies.Clear(); // .
app.UseForwardedHeaders(forwardedHeaderOptions);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();

    //Swagger
    app.UseSwagger(); // generate the json file for swagger
    var versionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    // for presentation for dropdown ()
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger"; // این می تواند خالی باشد یا "swagger" ()
        foreach (var desc in versionDescProvider.ApiVersionDescriptions)
        {
            // مسیر کامل شامل nginxPath ()
            c.SwaggerEndpoint($"{nginxPath}/swagger/{desc.GroupName}/swagger.json",
                $"Catalog.API - {desc.GroupName.ToUpper()}");
        }
    });
}
// بالای app.UsePathBase(...)
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();