namespace Common.Logging.Correlations;

public interface ICorrelationIdGenerator
{
    string Get();
    void Set(string correlationId);
}