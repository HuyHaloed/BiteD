namespace BiteDanceAPI.Domain.Entities;

public class Location : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public LocationType Type { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public bool EnableShift1 { get; set; }
    public bool EnableShift2 { get; set; }
    public bool EnableShift3 { get; set; }
    public bool EnableWeekday { get; set; }
    public bool EnableWeekend { get; set; }

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive && !value)
            {
                AddDomainEvent(new LocationDeactivatedEvent(this));
            }
            _isActive = value;
        }
    }

    // Parent
    public Supplier? Supplier { get; set; }
    public int? SupplierId { get; set; }

    // Children
    public ICollection<User> Admins { get; set; } = new List<User>();
}
