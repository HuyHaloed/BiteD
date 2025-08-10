namespace BiteDanceAPI.Domain.Entities;

public class DailyMenu : BaseEntity
{
    public DateOnly Date { get; set; }

    // Parent
    public required MonthlyMenu MonthlyMenu { get; set; }
    public required int MonthlyMenuId { get; set; }

    // Child
    public ICollection<ShiftMenu> ShiftMenus { get; set; } = new List<ShiftMenu>();
}
