using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Common.Logging.Correlations;

public class CorrelationIdGenerator(IHttpContextAccessor accessor) : ICorrelationIdGenerator
{
    private const string HeaderName = "X-Correlation-Id";
    private string? _cachedCorrelationId;

    public string Get()
    {
        if (_cachedCorrelationId != null)
            return _cachedCorrelationId;

        var context = accessor.HttpContext;
        if (context != null && context.Request.Headers.TryGetValue(HeaderName, out var correlationId))
        {
            _cachedCorrelationId = correlationId!;
            return _cachedCorrelationId;
        }

        _cachedCorrelationId = Guid.NewGuid().ToString("D");

        if (context != null)
        {
            context.Request.Headers[HeaderName] = _cachedCorrelationId;
        }

        return _cachedCorrelationId;
    }

    public void Set(string correlationId)
    {
        _cachedCorrelationId = correlationId;
        if (accessor.HttpContext != null) accessor.HttpContext.Request.Headers[HeaderName] = correlationId;
    }
}