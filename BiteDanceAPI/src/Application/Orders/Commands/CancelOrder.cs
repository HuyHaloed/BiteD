using BiteDanceAPI.Application.Common.Exceptions;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Orders.Commands;

[Authorize(DenySupplier = true)]
public record CancelOrderCommand(int OrderId) : IRequest;

public class CancelOrderCommandHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser,
    TimeProvider timeProvider
) : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        // var startDate = request.;
        // var endDate = startDate.AddDays(6);
 
        var order = await context
            .DailyOrders.Include(d => d.ShiftOrders)
            .FindOrNotFoundExceptionAsync(request.OrderId, cancellationToken);
        // var currentDate = startDate.AddDays(offset); 
        if (order.UserId != currentUser.Id)
        {
            throw new ForbiddenAccessException();
        }

        if (!order.CanBeCanceled(timeProvider.GetUtcNow().DateTime))
        {
            throw new InvalidOperationException("Cannot cancel the order after the deadline.");
        }
        // Console.WriteLine("Testtttttttt: " + timeProvider.GetUtcNow().DateTime);
        
        order.SetStatus(DailyOrderStatus.CanceledByUser);
        await context.SaveChangesAsync(cancellationToken);
    }
}
