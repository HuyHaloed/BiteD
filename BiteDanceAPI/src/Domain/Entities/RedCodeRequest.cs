namespace BiteDanceAPI.Domain.Entities;

public class RedCodeRequest : BaseAuditableEntity
{
    public required string FullName { get; set; }
    //public RedCodeRequesterRole Role { get; set; }
    public required string WorkEmail { get; set; }

    public DateOnly? checkInDate { get; set; } // Required for Contractor and Temp

    // Required for guests
    //public string? GuestTitle { get; set; }
    //public string? GuestContactNumber { get; set; }
    public string? GuestPurposeOfVisit { get; set; }
    //public DateOnly? GuestDateOfArrival { get; set; }

    public RedCodeRequestStatus Status { get; internal set; } = RedCodeRequestStatus.Submitted;
    public required string Note { get; set; }

    // Parent
    public required Location WorkLocation { get; set; }
    public int WorkLocationId { get; set; }

    public int OrderNumbers { get; set; }
    public required Department Department { get; set; } // contractor, temp
    public DepartmentChargeCode? DepartmentChargeCode { get; set; } // guest

    // Child
    public RedScanCode? RedScanCode { get; set; }

    public void Approve(RedScanCode scanCode, User admin, string? note)
    {
        Status = RedCodeRequestStatus.Approved;
        Note = note ?? Note;
        AddDomainEvent(new RedCodeRequestApprovedEvent(this, scanCode, admin));
    }

    public void Reject(User admin, string reason)
    {
        Status = RedCodeRequestStatus.Rejected;
        Note = reason;
        AddDomainEvent(new RedCodeRequestRejectedEvent(this, admin));
    }

    public void Disable(User admin)
    {
        Status = RedCodeRequestStatus.Disabled;
    }
}
