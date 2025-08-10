//CheckinRedQr.cs
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.ScanLogs.Commands;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Checkins.Commands;

[Authorize(RequireSupplier = true)]
public record CheckinRedQrCommand : IRequest<RedCheckinResult>
{
    public int LocationId { get; init; }
    public required string ScanCodeId { get; init; }
}

public class CheckinRedQrCommandHandler(
    IApplicationDbContext context,
    TimeProvider timeProvider,
    IUserService userService,
    IMediator mediator
) : IRequestHandler<CheckinRedQrCommand, RedCheckinResult>
{
    public async Task<RedCheckinResult> Handle(
        CheckinRedQrCommand request,
        CancellationToken cancellationToken
    )
    {
        RedCheckinResult result;
        
        try
        {
            // Admin
            var currentUser = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
            /*currentUser.AuthorizeAdminOrThrow(request.LocationId);*/
            
            // Location
            var location = await context.Locations.FindOrNotFoundExceptionAsync(
                request.LocationId,
                cancellationToken
            );
            if (!location.IsActive)
            {
                var errorResult = new RedCheckinResult { IsSuccess = false, Message = "Location not active." };
                await mediator.Send(new SubmitLogCommand 
                { 
                    UserId = "Unknown", // Red QR doesn't have specific UserId
                    LocationId = request.LocationId,
                    ScanCode = $"r:{request.ScanCodeId}",
                    LogMessage = errorResult.Message
                }, cancellationToken);
                return errorResult;
            }

            // Red scan code
            var scanCode = await context
                .RedScanCodes.Include(s => s.RedCodeRequest)
                .FirstOrDefaultAsync(s => s.RedCodeId == request.ScanCodeId, cancellationToken);

            if (scanCode == null)
            {
                var errorResult = new RedCheckinResult { IsSuccess = false, Message = "Scan code not found." };
                await mediator.Send(new SubmitLogCommand 
                { 
                    UserId = "Unknown",
                    LocationId = request.LocationId,
                    ScanCode = $"r:{request.ScanCodeId}",
                    LogMessage = errorResult.Message
                }, cancellationToken);
                return errorResult;
            }

            // Check if request is not disabled
            if (scanCode.RedCodeRequest.Status == RedCodeRequestStatus.Disabled)
            {
                var errorResult = new RedCheckinResult { IsSuccess = false, Message = "Scan code disabled." };
                await mediator.Send(new SubmitLogCommand 
                { 
                    UserId = scanCode.RedCodeRequest.FullName ?? "Unknown",
                    LocationId = request.LocationId,
                    ScanCode = $"r:{request.ScanCodeId}",
                    LogMessage = errorResult.Message
                }, cancellationToken);
                return errorResult;
            }

            // Check scan code is not expired
            var now = timeProvider.GetLocalNow();

            if (scanCode.ValidFrom > now)
            {
                var errorResult = new RedCheckinResult
                {
                    IsSuccess = false,
                    Message = "Scan code not yet valid."
                };
                await mediator.Send(new SubmitLogCommand 
                { 
                    UserId = scanCode.RedCodeRequest.FullName ?? "Unknown",
                    LocationId = request.LocationId,
                    ScanCode = $"r:{request.ScanCodeId}",
                    LogMessage = errorResult.Message
                }, cancellationToken);
                return errorResult;
            }

            if (scanCode.ValidTill < now)
            {
                var errorResult = new RedCheckinResult
                {
                    IsSuccess = false,
                    Message = "Scan code expired."
                };
                await mediator.Send(new SubmitLogCommand 
                { 
                    UserId = scanCode.RedCodeRequest.FullName ?? "Unknown",
                    LocationId = request.LocationId,
                    ScanCode = $"r:{request.ScanCodeId}",
                    LogMessage = errorResult.Message
                }, cancellationToken);
                return errorResult;
            }

            var listRedcodeCheckin = await context
                .RedCheckins.Where(c => c.ScanCodeId == scanCode.Id)
                .OrderByDescending(c => c.Datetime)
                .ToListAsync(cancellationToken);

            if (listRedcodeCheckin.Count >= scanCode.MaxNumScans)
            {
                var errorResult = new RedCheckinResult
                {
                    IsSuccess = false,
                    Message = "Maximum number of Scan of this code: " + listRedcodeCheckin.Count
                };
                await mediator.Send(new SubmitLogCommand 
                { 
                    UserId = scanCode.RedCodeRequest.FullName ?? "Unknown",
                    LocationId = request.LocationId,
                    ScanCode = $"r:{request.ScanCodeId}",
                    LogMessage = errorResult.Message
                }, cancellationToken);
                return errorResult;
            }

            // Latest checkin
            /*
            var latestCheckin = await context
                .RedCheckins.Where(c => c.ScanCodeId == scanCode.Id)
                .OrderByDescending(c => c.Datetime)
                .FirstOrDefaultAsync(cancellationToken);
            */
            // Guests can only scan once
            /*
            if (scanCode.RedCodeRequest.Role == RedCodeRequesterRole.Guests)
            {
                if (latestCheckin != null)
                {
                    return new RedCheckinResult { IsSuccess = false, Message = "Scan code used." };
                }
            }*/

            // Can't scan multiple times in a shift
            /*
            var currentTime = timeProvider.GetLocalNow().TimeOfDay;
            var currentShift = ShiftTypeExtensions.GetShift(currentTime);

            if (latestCheckin != null)
            {
                var latestCheckinShift = ShiftTypeExtensions.GetShift(latestCheckin.Datetime.TimeOfDay);
                if (latestCheckinShift == currentShift)
                {
                    return new RedCheckinResult
                    {
                        IsSuccess = false,
                        Message = "Already scanned in this shift."
                    };
                }
            }
            */
            
            // Create checkin
            var checkin = new RedCheckin
            {
                Location = location,
                LocationId = request.LocationId,
                Datetime = timeProvider.GetLocalNow(),
                ScanCode = scanCode,
                ScanCodeId = scanCode.Id,
            };
            
            context.RedCheckins.Add(checkin);
            await context.SaveChangesAsync(cancellationToken);

            result = new RedCheckinResult
            {
                IsSuccess = true,
                Message = "Check-in successful.",
                EmployeeName = scanCode.RedCodeRequest.FullName
            };
        }
        catch (Exception ex)
        {
            result = new RedCheckinResult
            {
                IsSuccess = false,
                Message = $"Check-in failed: {ex.Message}"
            };
        }

        // Submit log after processing
        await mediator.Send(new SubmitLogCommand 
        { 
            UserId = result.EmployeeName ?? "Unknown",
            LocationId = request.LocationId,
            ScanCode = $"r:{request.ScanCodeId}",
            LogMessage = result.Message
        }, cancellationToken);

        return result;
    }
}