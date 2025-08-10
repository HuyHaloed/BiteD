namespace BiteDanceAPI.Domain.Entities;

public class ShiftMenu : BaseEntity
{
    public ShiftType Shift { get; set; }

    // Parent
    public required DailyMenu DailyMenu { get; set; }

    // Child
    public List<Dish> Dishes { get; set; } = [];
}
