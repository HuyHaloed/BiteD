namespace BiteDanceAPI.Domain.Entities;

public class DailyOrder : BaseAuditableEntity
{
    public DailyOrderStatus Status { get; internal set; }
    public DateOnly Date { get; set; }

    // Parent
    public required User User { get; set; }
    public required string UserId { get; set; }

    public required Location Location { get; set; }
    public int LocationId { get; set; }

    // Child
    public ICollection<ShiftOrder> ShiftOrders { get; set; } = new List<ShiftOrder>();

    public bool CanBeCanceled( DateTime currentTime)
    {
        // Cancel deadline is 12:00 PM the day before the order date
        if (Date.DayOfWeek == DayOfWeek.Monday)
        {
            var previousFriday = Date.AddDays(-3);
            var fridayDeadline = new DateTime(
                previousFriday.Year,
                previousFriday.Month,
                previousFriday.Day,
                16,
                0,
                0,
                DateTimeKind.Utc
            ).AddHours(-7); // Convert to GMT+7

            return currentTime < fridayDeadline;
        }
        var cancelDeadline = new DateTime( //
            Date.Year,
            Date.Month,
            Date.Day,
            16,
            0,
            0,
            DateTimeKind.Utc
        ).AddDays(-1).AddHours(-7); // Convert to GMT+7

        return currentTime < cancelDeadline;
    
    }

    public void SetStatus(DailyOrderStatus status)
    {
        Status = status;
        foreach (var shiftOrder in ShiftOrders)
        {
            shiftOrder.Status = status switch
            {
                DailyOrderStatus.Ordered => ShiftOrderStatus.Ordered,
                DailyOrderStatus.CanceledByUser => ShiftOrderStatus.CanceledByUser,
                DailyOrderStatus.CanceledBySystem => ShiftOrderStatus.CanceledBySystem,
                _ => throw new ArgumentException("Invalid status")
            };
        }
    }
}
