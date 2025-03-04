using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("CorsPolicy", policy =>
//     {
//         policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
//     });
// });
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
builder.Services.AddOcelot();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
// app.UseCors("CorsPolicy");
// app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    if (endpoints == null) throw new ArgumentNullException(nameof(endpoints));
    endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello Ocelot"); });
});

await app.UseOcelot();
await app.RunAsync();