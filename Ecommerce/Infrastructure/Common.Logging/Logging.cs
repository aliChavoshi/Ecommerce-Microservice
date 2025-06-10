using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging;

public static class Logging
{
    public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
        (context, loggerConfiguration) =>
        {
            var env = context.HostingEnvironment;
            loggerConfiguration.MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Override("Default", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .WriteTo.Console();

            if (context.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration
                    .MinimumLevel.Override("Catalog", LogEventLevel.Debug)
                    .MinimumLevel.Override("Basket", LogEventLevel.Debug)
                    .MinimumLevel.Override("Discount", LogEventLevel.Debug)
                    .MinimumLevel.Override("Ordering", LogEventLevel.Debug)
                    .MinimumLevel.Override("Ocelot.ApiGateways", LogEventLevel.Debug)
                    .MinimumLevel.Override("Ocelot", LogEventLevel.Debug)
                    .MinimumLevel.Override("Common", LogEventLevel.Debug);
            }


            //Elastic Search
            var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            if (!string.IsNullOrEmpty(elasticUrl))
            {
                loggerConfiguration.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(elasticUrl))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                        IndexFormat = "ecommerce-Logs-{0:yyyy.MM.dd}",
                        MinimumLogEventLevel = LogEventLevel.Debug
                    });
            }
        };
}