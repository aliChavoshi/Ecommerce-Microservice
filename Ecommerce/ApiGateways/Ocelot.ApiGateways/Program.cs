using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
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
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://host.docker.internal:44300"; // نکته مهم
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://localhost:44300", // همان مقدار token issuer
            ValidateAudience = true,
            ValidAudience = "Catalog",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        // فقط در صورت نیاز به عبور از TLS Self-Signed:
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
        };
    });

//End Identity
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