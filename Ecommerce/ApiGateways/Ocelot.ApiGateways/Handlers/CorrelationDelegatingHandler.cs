using Common.Logging.Correlations;

namespace Ocelot.ApiGateways.Handlers;

public class CorrelationDelegatingHandler(ICorrelationIdGenerator correlation) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = correlation.Get();
        if (!request.Headers.Contains("X-Correlation-Id"))
        {
            request.Headers.Add("X-Correlation-Id", correlationId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
