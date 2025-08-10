namespace BiteDanceAPI.Domain.Events;

public class RedCodeRequestApprovedEvent(
    RedCodeRequest redCodeRequest,
    RedScanCode scanCode,
    User approver
) : BaseEvent
{
    public RedCodeRequest RedCodeRequest { get; } = redCodeRequest;
    public RedScanCode ScanCode { get; } = scanCode;
    public User Approver { get; set; } = approver;
}
