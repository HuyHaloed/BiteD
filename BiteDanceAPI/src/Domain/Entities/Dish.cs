namespace BiteDanceAPI.Domain.Entities;

public class Dish : BaseEntity
{
    public required string Name { get; set; }
    public DishType Type { get; set; }

    // Parent
    public required Location Location { get; set; }
    public int LocationId { get; set; }
    public required Supplier Supplier { get; set; }
    public int SupplierId { get; set; }
}
