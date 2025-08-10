namespace BiteDanceAPI.Domain.Events;

public class LocationDeactivatedEvent(Location location) : BaseEvent
{
    public Location Location { get; } = location;
}
