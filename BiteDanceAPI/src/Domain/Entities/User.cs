namespace BiteDanceAPI.Domain.Entities;

public class User
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsManager { get; set; }
    public bool IsSupplier { get; set; }
    public bool AllowNotifications { get; set; } = false;


    // Parent
    public Location? PreferredLocation { get; set; }
    public int? PreferredLocationId { get; set; }

    // Child
    public ICollection<Location> AssignedLocations { get; set; } = new List<Location>();
    
}
