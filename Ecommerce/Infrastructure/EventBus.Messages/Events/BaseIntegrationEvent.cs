namespace EventBus.Messages.Events;

public class BaseIntegrationEvent
{
    public string CorrelationId { get; set; }
    public DateTime CreationDate { get; set; }

    public BaseIntegrationEvent()
    {
        CorrelationId = Guid.NewGuid().ToString();
        CreationDate = DateTime.Now;
    }

    public BaseIntegrationEvent(Guid correlationId, DateTime creationDate)
    {
        CorrelationId = correlationId.ToString();
        CreationDate = creationDate;
    }
}