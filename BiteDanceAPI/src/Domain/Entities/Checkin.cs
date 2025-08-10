namespace BiteDanceAPI.Domain.Entities;

public abstract class Checkin : BaseAuditableEntity
{
    public CodeType Type { get; set; }
    public DateTimeOffset Datetime { get; set; }

    // Parent
    public required Location Location { get; set; }
    public int LocationId { get; set; }

    // Required for Blue, Green
    public User? User { get; set; }
    public string? UserId { get; set; }
}

public class BlueCheckin : Checkin
{
    public ShiftType Shift { get; set; }
}

public class GreenCheckin : Checkin
{
    // Parent
    public required ShiftOrder ShiftOrder { get; set; }
    public int ShiftOrderId { get; set; }
}

public class RedCheckin : Checkin
{
    // Parent
    public required ScanCode ScanCode { get; set; }
    public int ScanCodeId { get; set; }
}

public class PurpleCheckin : Checkin
{
    // Parent
    public required ScanCode ScanCode { get; set; }
    public int ScanCodeId { get; set; }
}
