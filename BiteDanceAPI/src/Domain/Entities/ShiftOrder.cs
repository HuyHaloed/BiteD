namespace BiteDanceAPI.Domain.Entities;

public class ShiftOrder : BaseAuditableEntity
{
    // Parent
    public required ShiftOrderStatus Status { get; set; }
    public required DailyOrder DailyOrder { get; set; }

    public required ShiftMenu ShiftMenu { get; set; }
    public required ShiftType ShiftType { get; set; } // Derived from shift menu
    public required Location Location { get; set; }
    public int LocationId { get; set; }

    // Child
    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
