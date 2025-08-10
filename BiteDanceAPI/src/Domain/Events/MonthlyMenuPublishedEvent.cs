namespace BiteDanceAPI.Domain.Events;

public class MonthlyMenuPublishedEvent(MonthlyMenu monthlyMenu) : BaseEvent
{
    public MonthlyMenu MonthlyMenu { get; } = monthlyMenu;
}
