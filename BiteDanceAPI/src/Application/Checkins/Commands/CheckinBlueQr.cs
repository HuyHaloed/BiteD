//CheckinBlueQr.cs
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.ScanLogs.Commands;
using BiteDanceAPI.Domain.Constants;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Checkins.Commands;

[Authorize(RequireSupplier = true)]
public record CheckinBlueQrCommand : IRequest<BlueCheckinResult>
{
    public int LocationId { get; init; }
    public required string UserId { get; init; }
}

public class CheckinBlueQrCommandHandler(
    IApplicationDbContext context,
    TimeProvider timeProvider,
    IUserService userService,
    IMediator mediator
) : IRequestHandler<CheckinBlueQrCommand, BlueCheckinResult>
{
    public async Task<BlueCheckinResult> Handle(
        CheckinBlueQrCommand request,
        CancellationToken cancellationToken
    )
    {
        BlueCheckinResult result;
        
        try
        {

        // Admin
        var currentUser = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        /*currentUser.AuthorizeAdminOrThrow(request.LocationId);*/
        
        // Actual user
        var user = await context.Users.FindAsync([request.UserId], cancellationToken);
        if (user == null)
        {
            var errorResult = new BlueCheckinResult { IsSuccess = false, Message = "User does not exist." };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"b:{request.UserId}",
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
            var errorResult = new BlueCheckinResult { IsSuccess = false, Message = "Location not active." };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"b:{request.UserId}",
                LogMessage = errorResult.Message
            }, cancellationToken);
            return errorResult;
        }

        // Check max scans per day
        var today = timeProvider.GetLocalNow().Date;
        if (
            await context.BlueCheckins.CountAsync(
                c => c.UserId == request.UserId && c.Datetime.Date == today,
                cancellationToken
            ) >= BlueCheckinConst.MaxScansPerDay
        )
        {
            var errorResult = new BlueCheckinResult
            {
                IsSuccess = false,
                Message = $"Exceeded {BlueCheckinConst.MaxScansPerDay} blue checkins per day."
            };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"b:{request.UserId}",
                LogMessage = errorResult.Message
            }, cancellationToken);
            return errorResult;
        }

        // Check max scans per week
        var startOfWeek = StartOfWeek(timeProvider.GetLocalNow(), DayOfWeek.Monday);
        var endOfWeek = startOfWeek.AddDays(7);
        if (
            await context.BlueCheckins.CountAsync(
                c =>
                    c.UserId == request.UserId
                    && c.Datetime >= startOfWeek
                    && c.Datetime < endOfWeek,
                cancellationToken
            ) >= BlueCheckinConst.MaxScansPerWeek
        )
        {
            var errorResult = new BlueCheckinResult
            {
                IsSuccess = false,
                Message = $"Exceeded {BlueCheckinConst.MaxScansPerWeek} blue checkins per week."
            };
            await mediator.Send(new SubmitLogCommand 
            { 
                UserId = request.UserId, 
                LocationId = request.LocationId,
                ScanCode = $"b:{request.UserId}",
                LogMessage = errorResult.Message
            }, cancellationToken);
            return errorResult;
        }

        // Create checkin
        var currentTime = timeProvider.GetLocalNow().TimeOfDay;
        var checkin = new BlueCheckin
        {
            Location = location,
            LocationId = request.LocationId,
            User = user,
            UserId = request.UserId,
            Datetime = timeProvider.GetLocalNow(),
            Shift = ShiftTypeExtensions.GetShift(currentTime) ?? throw new InvalidOperationException("Shift not found.")
        };

        context.BlueCheckins.Add(checkin);
        await context.SaveChangesAsync(cancellationToken);

        result = new BlueCheckinResult
        {
            IsSuccess = true,
            Message = "Check-in successful.",
            EmployeeName = user.Name
        };
        }
        catch (Exception ex)
        {
            result = new BlueCheckinResult
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
            ScanCode = $"b:{request.UserId}",
            LogMessage = result.Message
        }, cancellationToken);

        return result;
    }

    private static DateTimeOffset StartOfWeek(DateTimeOffset date, DayOfWeek startOfWeek)
    {
        int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}