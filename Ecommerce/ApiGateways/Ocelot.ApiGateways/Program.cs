using Common.Logging;
using Common.Logging.Correlations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.ApiGateways.Handlers;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
// -----------------------------
// اضافه شده برای لاگ‌گیری
// -----------------------------
builder.Host.UseSerilog(Logging.ConfigureLogger);

// -----------------------------
// بارگذاری پیکربندی Ocelot
// -----------------------------
builder.Host.ConfigureAppConfiguration((env, config) =>
{
    var environmentName = env.HostingEnvironment?.EnvironmentName ?? "Development";
    config.AddJsonFile($"ocelot.{environmentName}.json", optional: true, reloadOnChange: true);
});

// -----------------------------
// سرویس‌های پایه
// -----------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// افزودن Correlation ID
// -----------------------------
builder.Services.AddTransient<ICorrelationIdGenerator, CorrelationIdGenerator>();
builder.Services.AddTransient<CorrelationDelegatingHandler>();
builder.Services.AddHttpContextAccessor();

// -----------------------------
// تنظیمات CORS برای فرانت‌اند
// -----------------------------
var corsPolicyName = "AllowAllOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// -----------------------------
// افزودن Ocelot و کش و Handler سفارشی
// -----------------------------
builder.Services
    .AddOcelot()
    .AddDelegatingHandler<CorrelationDelegatingHandler>()
    .AddCacheManager(x => x.WithDictionaryHandle());

// -----------------------------
// احراز هویت JWT با IdentityServer
// // -----------------------------
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer("EShoppingGatewayAuthScheme", options =>
//     {
//         options.Authority = "https://localhost:9009";
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateAudience = true,
//             ValidAudiences = new[] { "Catalog", "Basket", "EShoppingGateway" }
//         };
//         options.RequireHttpsMetadata = false;
//     });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("CatalogAuthScheme", options =>
    {
        // options.Authority = "https://localhost:9009";
        options.Authority = "https://host.docker.internal:9009";
        options.Audience = "Catalog";
        options.RequireHttpsMetadata = false; //dev
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://host.docker.internal:9009"
        };
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Log.Error(ctx.Exception, "JWT Authentication failed for CatalogAuthScheme");
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Log.Warning("JWT Challenge triggered for CatalogAuthScheme. Error: {Error}, Description: {ErrorDescription}",
                    ctx.Error, ctx.ErrorDescription);
                return Task.CompletedTask;
            },
            OnForbidden = ctx =>
            {
                Log.Warning("JWT Forbidden for CatalogAuthScheme. User: {User}", ctx.HttpContext.User?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer("BasketAuthScheme", options =>
    {
        //options.Authority = "https://localhost:9009";
        options.Authority = "https://host.docker.internal:9009";
        options.Audience = "Basket";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://host.docker.internal:9009"
        };
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Log.Error(ctx.Exception, "JWT Authentication failed for BasketAuthScheme");
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Log.Warning("JWT Challenge triggered for BasketAuthScheme. Error: {Error}, Description: {ErrorDescription}",
                    ctx.Error, ctx.ErrorDescription);
                return Task.CompletedTask;
            },
            OnForbidden = ctx =>
            {
                Log.Warning("JWT Forbidden for BasketAuthScheme. User: {User}", ctx.HttpContext.User?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

// -----------------------------
// ساخت اپلیکیشن و میدلورها
// -----------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
// فعال‌سازی CORS
app.UseCors(corsPolicyName);
// فعال‌سازی احراز هویت و مجوز
app.UseAuthentication();
app.UseAuthorization();
// Middleware برای افزودن CorrelationId
app.AddCorrelationIdMiddleware();
app.MapControllers();
// فقط برای تست لاگ
app.MapGet("/", () => "Hello Ocelot");

app.Use(async (context, next) =>
{
    var correlation = context.RequestServices.GetRequiredService<ICorrelationIdGenerator>();
    var correlationId = correlation.Get();
    Log.Information("Middleware CorrelationId In Ocelot: {correlationId}", correlationId);

    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unhandled exception in Ocelot");
        throw;
    }
});

// اجرای Ocelot به عنوان API Gateway
await app.UseOcelot();
await app.RunAsync();