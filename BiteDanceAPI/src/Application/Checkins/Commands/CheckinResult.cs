using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Checkins.Commands;

public class CheckinResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? StatusType { get; set; }
    public virtual CodeType CodeType { get; set; }
    public IReadOnlyCollection<string> DishNames { get; set; } = new List<string>();
    public string EmployeeName { get; set; } = string.Empty;
    public DateTimeOffset ScannedAt { get; set; } = DateTimeOffset.Now;
}

public class GreenCheckinResult : CheckinResult
{
    public override CodeType CodeType { get; set; } = CodeType.Green;
}

public class BlueCheckinResult : CheckinResult
{
    public override CodeType CodeType { get; set; } = CodeType.Blue;
}

public class RedCheckinResult : CheckinResult
{
    public override CodeType CodeType { get; set; } = CodeType.Red;
}
