using System.Reflection;
using Discount.API.Services;
using Discount.Application.Commands;
using Discount.Application.Mapper;
using Discount.Core.Interfaces;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Mapper
builder.Services.AddAutoMapper(typeof(DiscountProfile));
//Register Mediatr
var assemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(CreateDiscountCommandHandler).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
//DI
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
//GRPC
builder.Services.AddGrpc();
// 
var app = builder.Build();
//Migration
app.MigrateDatabase<Program>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapGrpcService<DiscountService>();
app.MapGet("/",
    async context =>
    {
        await context.Response.WriteAsync(
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
    });

app.Run();