using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Enums;
using BiteDanceAPI.Domain.Events;
using Microsoft.Extensions.Logging;

namespace BiteDanceAPI.Application.Suppliers.EventHandlers;

public class SupplierDeactivatedEventHandler(
    IApplicationDbContext context,
    ILogger<SupplierDeactivatedEventHandler> logger
) : INotificationHandler<SupplierDeactivatedEvent>
{
    public async Task Handle(
        SupplierDeactivatedEvent notification,
        CancellationToken cancellationToken
    )
    {
        var locationIds = await context
            .Locations.Where(l => l.SupplierId == notification.Supplier.Id)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Deactivating orders of locations: {@locationIds}", locationIds);

        await context
            .DailyOrders.Where(x => locationIds.Contains(x.LocationId))
            .ExecuteUpdateAsync(
                s => s.SetProperty(o => o.Status, DailyOrderStatus.CanceledBySystem),
                cancellationToken
            );
        await context
            .ShiftOrders.Where(x => locationIds.Contains(x.LocationId))
            .ExecuteUpdateAsync(
                s => s.SetProperty(o => o.Status, ShiftOrderStatus.CanceledBySystem),
                cancellationToken
            );
    }
}
