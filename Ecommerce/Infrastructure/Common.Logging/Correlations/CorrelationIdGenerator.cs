namespace Common.Logging.Correlations;

public class CorrelationIdGenerator : ICorrelationIdGenerator
{
    private string _correlationId = Guid.NewGuid().ToString("D");

    public string Get()
    {
        return _correlationId;
    }

    public void Set(string correlationId)
    {
        _correlationId = correlationId;
    }
}