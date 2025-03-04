namespace Ocelot.ApiGateways;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var build = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(((context, config) =>
        {
            config.AddJsonFile($"ocelot.{context.HostingEnvironment.EnvironmentName}.json", true, true);
        })).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        return build;
    }
}