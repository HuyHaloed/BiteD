namespace BiteDanceAPI.Domain.Entities;

public abstract class ScanCode : BaseAuditableEntity
{
    public CodeType Type { get; internal set; }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset ValidTill { get; set; }
    public bool IsDisabled { get; internal set; }
    public string DisabledReason { get; internal set; } = string.Empty;

    public void SetType(CodeType type)
    {
        if (type is not CodeType.Red and not CodeType.Purple)
        {
            throw new UnsupportedScanCodeTypeException(type);
        }

        this.Type = type;
    }

    public void Disable(string reason)
    {
        // TODO: validate not disabled, reason not null or empty
        IsDisabled = true;
        DisabledReason = reason;
    }
}

public class RedScanCode : ScanCode
{
    // Random generated string, prevent guessing
    public required string RedCodeId { get; set; }
    public int MaxNumScans { get; set; }
    // Parent
    public required RedCodeRequest RedCodeRequest { get; set; }
    public int RedCodeRequestId { get; set; }
    public int OrderNumbers { get; set; }

    public required Location Location { get; set; }
    public int LocationId { get; set; }
}

public class PurpleScanCode : ScanCode
{
    // TODO: private set, validation
    public int MaxNumScans { get; set; }

    // Parent
    public required User Issuer { get; set; }
    public required string IssuerId { get; set; }
}
