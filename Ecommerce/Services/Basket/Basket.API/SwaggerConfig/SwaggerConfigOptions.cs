using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Basket.API.SwaggerConfig;

public class SwaggerConfigOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var desc in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(desc.GroupName, new OpenApiInfo
            {
                //this is for header of the swagger documentation
                Title = "Basket APIs",
                Version = desc.ApiVersion.ToString(),
                Contact = new OpenApiContact()
                {
                    Email = "ali@gmail.com",
                    Name = "Ali Chavoshi",
                },
                Description = "This is the Basket Microservice",
                License = new OpenApiLicense()
                {
                    Name = "MIT"
                }
            });
        }
    }
}