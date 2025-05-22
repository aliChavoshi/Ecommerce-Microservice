using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Catalog.API.SwaggerConfig;

public class SwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var desc in context.ApiDescriptions)
        {
            if (desc.ParameterDescriptions.Any(p => p is { Name: "version", Source.Id: "Query" }))
            {
                if (desc.ParameterDescriptions.Any(p => p is { Name: "version", Source.Id: "Query" }))
                    swaggerDoc.Paths.Remove($"/{desc.RelativePath?.TrimEnd('/')}");
            }
        }
    }
}