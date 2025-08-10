namespace BiteDanceAPI.Domain.Events;

public class RedCodeRequestSubmittedEvent(RedCodeRequest redCodeRequest) : BaseEvent
{
    public RedCodeRequest RedCodeRequest { get; } = redCodeRequest;
}
