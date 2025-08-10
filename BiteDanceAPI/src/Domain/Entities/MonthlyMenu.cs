namespace BiteDanceAPI.Domain.Entities;

public class MonthlyMenu : BaseEntity
{
    public int Year { get; set; }
    public int Month { get; set; }
    public bool IsPublished { get; set; }

    // Parent
    public required Location Location { get; set; }
    public required int LocationId { get; set; }
    public required Supplier Supplier { get; set; }
    public int SupplierId { get; set; }

    // Child
    public ICollection<DailyMenu> DailyMenus { get; set; } = new List<DailyMenu>();
}
