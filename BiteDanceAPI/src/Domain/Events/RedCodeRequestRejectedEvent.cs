namespace BiteDanceAPI.Domain.Events;

public class RedCodeRequestRejectedEvent(RedCodeRequest redCodeRequest, User approver) : BaseEvent
{
    public RedCodeRequest RedCodeRequest { get; } = redCodeRequest;
    public User Approver { get; } = approver;
}
