using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Security.Cryptography.X509Certificates;
using Ocelot.ApiGateways.Handlers;

var builder = WebApplication.CreateBuilder(args);

//ocelot configuration
builder.Host.ConfigureAppConfiguration((env, config) =>
{
    var environmentName = env.HostingEnvironment.EnvironmentName ?? "Development";
    if (config != null) config.AddJsonFile($"ocelot.{environmentName}.json", true, true);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot().AddDelegatingHandler<ForwardTokenHandler>()
    .AddCacheManager(x => x.WithDictionaryHandle());
//Identity Server
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:44300";
        options.RequireHttpsMetadata = false;
        options.MetadataAddress = "https://localhost:44300/.well-known/openid-configuration";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://localhost:44300",
            ValidateAudience = true,
            ValidAudience = "Catalog", // فقط یک audience
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // اگر کلیدها به صورت دستی دریافت می‌شوند:
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                // 1. دریافت JWKS از IdentityServer
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
                };
                using var httpClient = new HttpClient(handler);

                // دریافت JWKS از آدرس jwks_uri
                var jwksUri = "https://localhost:44300/.well-known/openid-configuration/jwks";
                var jwksResponse = httpClient.GetAsync(jwksUri).GetAwaiter().GetResult();
                jwksResponse.EnsureSuccessStatusCode();

                // پردازش پاسخ
                var jwksJson = jwksResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var jwks = new JsonWebKeySet(jwksJson);

                // فیلتر کلیدها بر اساس kid
                return jwks.Keys.Where(k => k.Kid == kid).ToList();
            }
        };

        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello Ocelot");

await app.UseOcelot();
await app.RunAsync();