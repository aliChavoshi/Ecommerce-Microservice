using Common.Logging;
using Common.Logging.Correlations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.ApiGateways.Handlers;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
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
// -----------------------------
// اگر قصد داشته باشی بعضی مسیرها (مثل Catalog) بدون احراز هویت باشن
// این بخش همچنان لازم هست برای مسیرهای محافظت‌شده دیگر.
// اما روت‌های خاص را می‌تونی در ocelot.json از Authentication حذف کنی.
var authScheme = "EShoppingGatewayAuthScheme";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(authScheme, options =>
    {
        options.Authority = "http://identityserveraspnetidentity:8080"; // آدرس سرور احراز هویت
        options.Audience = "EShoppingGateway"; // نام مخاطب
        options.RequireHttpsMetadata = false;
    });

// -----------------------------
// ساخت اپلیکیشن و میدلورها
// -----------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// فعال‌سازی CORS
app.UseCors(corsPolicyName);

// Middleware برای افزودن CorrelationId
app.AddCorrelationIdMiddleware();

app.UseRouting();

// فعال‌سازی احراز هویت و مجوز
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// فقط برای تست لاگ
app.MapGet("/", () => "Hello Ocelot");

app.Use(async (context, next) =>
{
    var correlation = context.RequestServices.GetRequiredService<ICorrelationIdGenerator>();
    var correlationId = correlation.Get();
    Log.Information("Middleware CorrelationId In Ocelot: {correlationId}", correlationId);

    await next();
});

// اجرای Ocelot به عنوان API Gateway
await app.UseOcelot();
await app.RunAsync();
