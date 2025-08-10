namespace BiteDanceAPI.Domain.Entities;

public class Supplier : BaseEntity
{
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string CertificateOfBusinessNumber { get; set; }
    public DateOnly ContractStartDate { get; set; }
    public DateOnly ContractEndDate { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string PicName { get; set; }
    public required string PicPhoneNumber { get; set; }
    public required string BaseLocation { get; set; }
    public string? UserId { get; set; }
    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive && !value)
            {
                AddDomainEvent(new SupplierDeactivatedEvent(this));
            }
            _isActive = value;
        }
    }

    // Child
    public ICollection<Location> AssignedLocations { get; set; } = new List<Location>();
}
