using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Enums;
using BiteDanceAPI.Domain.Events;

namespace BiteDanceAPI.Application.Locations.EventHandlers;

public class LocationDeactivatedEventHandler(IApplicationDbContext context)
    : INotificationHandler<LocationDeactivatedEvent>
{
    public async Task Handle(
        LocationDeactivatedEvent notification,
        CancellationToken cancellationToken
    )
    {
        await context
            .DailyOrders.Where(x => x.LocationId == notification.Location.Id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(o => o.Status, DailyOrderStatus.CanceledBySystem),
                cancellationToken
            );
        await context
            .ShiftOrders.Where(x => x.LocationId == notification.Location.Id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(o => o.Status, ShiftOrderStatus.CanceledBySystem),
                cancellationToken
            );
    }
}
