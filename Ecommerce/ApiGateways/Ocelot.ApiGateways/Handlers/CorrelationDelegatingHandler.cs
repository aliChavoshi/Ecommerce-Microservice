using Common.Logging.Correlations;
using Serilog;

namespace Ocelot.ApiGateways.Handlers;

public class CorrelationDelegatingHandler(
    ICorrelationIdGenerator correlation,
    ILogger<CorrelationDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("🔥🔥 CorrelationDelegatingHandler is executing 🔥🔥");

        var correlationId = correlation.Get();
        if (!request.Headers.Contains("X-Correlation-Id"))
        {
            request.Headers.Add("X-Correlation-Id", correlationId);
        }

        logger.LogInformation("[Ocelot] correlationId: {correlationId}", correlationId);

        return await base.SendAsync(request, cancellationToken);
    }
}