//CheckinGreenQr.cs
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.ScanLogs.Commands;
using BiteDanceAPI.Domain.Constants;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Checkins.Commands;

[Authorize(RequireSupplier = true)] // Phải là supplier thì mới có thể qr-scanner

public record CheckinGreenQrCommand : IRequest<GreenCheckinResult>
{
    public int LocationId { get; init; }
    public required string UserId { get; init; } // required truyền vào từ frontend QRScannerPage có 2 para
}

public class CheckinGreenQrCommandHandler(
    IApplicationDbContext context, 
    TimeProvider timeProvider,
    IUserService userService,   // hiện giờ commandhandler này chỉ dùng để lấy admin user hiện tại, và không làm gì cả
    IMediator mediator      // để gửi về log sau khi checkin thành công
) : IRequestHandler<CheckinGreenQrCommand, GreenCheckinResult>
{
    public async Task<GreenCheckinResult> Handle(
        CheckinGreenQrCommand request,
        CancellationToken cancellationToken
    )
    {
        GreenCheckinResult result;
        
        try
        {

        // Admin
        var currentUser = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
       /*currentUser.AuthorizeAdminOrThrow(request.LocationId);*/

        // Actual user
        var user = await context.Users.FindAsync([request.UserId], cancellationToken);
        if (user == null)
        {
            var errorResult = new GreenCheckinResult { IsSuccess = false, Message = "User not found." };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = "g:" + request.UserId,
                LogMessage = errorResult.Message
            }, cancellationToken);
            return errorResult;
        }

        // Location
        var location = await context.Locations.FindOrNotFoundExceptionAsync(
            request.LocationId,
            cancellationToken
        );
        
        if (!location.IsActive)
        {
            var errorResult = new GreenCheckinResult
            {
                IsSuccess = false,
                Message = "Location is not active."
            };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"g:{request.UserId}",
                LogMessage = errorResult.Message
            }, cancellationToken);
            return errorResult;
        }

        var currentTime = timeProvider.GetLocalNow().TimeOfDay;
        DateOnly orderDate = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        // Back one day if the current time is after 24:00 and before the first shift starts (still in shift 3 of previous day)
        if (currentTime > TimeSpan.FromHours(24) && currentTime < ShiftConst.Shift1Start)
        {
            orderDate = orderDate.AddDays(-1);
        }

        var dailyOrder = await context
            .DailyOrders.Include(o => o.ShiftOrders)
            .ThenInclude(s => s.Dishes)
            .OrderByDescending(order => order.Created) // Take latest order
            .FirstOrDefaultAsync(
                o =>
                    o.UserId == request.UserId
                    && o.LocationId == request.LocationId
                    && o.Date == orderDate,
                cancellationToken
            );
        if (dailyOrder == null)
        {
            var errorResult = new GreenCheckinResult { IsSuccess = false, Message = "Daily order not found." }; 
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"g:{request.UserId}",
                LogMessage = errorResult.Message
            }, cancellationToken);
            return errorResult;
        }
        var currentShift = (location.Type == LocationType.HeadOffice
            ? ShiftTypeExtensions.GetShiftHeadOffice(currentTime)
            : ShiftTypeExtensions.GetShift(currentTime)) ?? ShiftType.Shift2;

        var shiftOrder = dailyOrder.ShiftOrders.FirstOrDefault(so => so.ShiftType == currentShift);
        if (shiftOrder == null)
        {
            var errorResult = new GreenCheckinResult { IsSuccess = false, Message = "Shift order not found." };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"g:{request.UserId}",
                LogMessage = errorResult.Message
            }, cancellationToken);
            return errorResult;
        }

    
        if (shiftOrder.Status is ShiftOrderStatus.CanceledBySystem or ShiftOrderStatus.CanceledByUser)
        {
            var errorResult = new GreenCheckinResult
            {
                IsSuccess = false,
                Message = "Order has been canceled."
                
            };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"g:{request.UserId}",
                LogMessage = errorResult.Message,
                ShiftOrderId = shiftOrder.Id
            }, cancellationToken);
            return errorResult;
        }

        if (shiftOrder.Status is ShiftOrderStatus.Scanned)
        {
            var errorResult = new GreenCheckinResult
            {
                IsSuccess = false,
                Message = "Order has already been scanned."
            };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"g:{request.UserId}",
                LogMessage = errorResult.Message,
                ShiftOrderId = shiftOrder.Id
            }, cancellationToken);
            return errorResult;
        }

        // Update shift order status
        shiftOrder.Status = ShiftOrderStatus.Scanned;

        // Create checkin
        var checkin = new GreenCheckin
        {
            Location = location,
            LocationId = request.LocationId,
            User = user,
            UserId = request.UserId,
            Datetime = timeProvider.GetLocalNow(),
            ShiftOrder = shiftOrder,
            ShiftOrderId = shiftOrder.Id
        };

        // Save changes
        context.GreenCheckins.Add(checkin);
        await context.SaveChangesAsync(cancellationToken);

        result = new GreenCheckinResult
        {
            IsSuccess = true,
            Message = "Check-in successful.",
            EmployeeName = user.Name,
            DishNames = shiftOrder.Dishes.Select(d => d.Name).ToList()
        };
        }
        catch (Exception ex)
        {
            result = new GreenCheckinResult
            {
                IsSuccess = false,
                Message = $"Check-in failed: {ex.Message}"
            };
        }

        // Submit log after processing
        await mediator.Send(new SubmitLogCommand 
        { 
            UserId = request.UserId, 
            LocationId = request.LocationId,
            ScanCode = $"g:{request.UserId}",
            LogMessage = result.Message,
            ShiftOrderId = result.IsSuccess ? 
                (await context.DailyOrders
                    .Include(o => o.ShiftOrders)
                    .Where(o => o.UserId == request.UserId && o.LocationId == request.LocationId)
                    .OrderByDescending(o => o.Created)
                    .FirstOrDefaultAsync(cancellationToken))
                    ?.ShiftOrders.FirstOrDefault()?.Id : null
        }, cancellationToken);

        return result;
    }
}