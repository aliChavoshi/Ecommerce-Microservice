var builder = DistributedApplication.CreateBuilder(args);

// وابستگی‌ها
var password = builder.AddParameter("sqlServerPassword", "Rahul1234567");
var sqlServer = builder.AddSqlServer("sqlServer", password: password);
var mongoDb = builder.AddMongoDB("mongoDB");
var postgresDb = builder.AddPostgres("postgres");
var redis = builder.AddRedis("redis");
var rabbit = builder.AddRabbitMQ("rabbitMQ");

// سرویس‌ها
var discount = builder.AddProject<Projects.Discount_API>("discount")
    .WithReference(postgresDb);

var basket = builder.AddProject<Projects.Basket_API>("basket")
    .WithReference(redis)
    .WithReference(rabbit)
    .WithReference(discount); // 👈 Basket به Discount متصل است (gRPC)

var catalog = builder.AddProject<Projects.Catalog_API>("catalog")
    .WithReference(mongoDb);

var ordering = builder.AddProject<Projects.Ordering_API>("ordering")
    .WithReference(sqlServer)
    .WithReference(rabbit);

var ocelot = builder.AddProject<Projects.Ocelot_ApiGateways>("apigateway");

var identity = builder.AddProject<Projects.IdentityServerAspNetIdentity>("identity");

builder.Build().Run();