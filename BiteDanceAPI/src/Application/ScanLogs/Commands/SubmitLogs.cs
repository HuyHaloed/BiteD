//SubmitLogs.cs
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.ScanLogs.Commands;

[Authorize(RequireSupplier = true)]

public record SubmitLogCommand : IRequest
{
    public required string UserId { get; init; }
    public int LocationId { get; init; }
    public required string ScanCode { get; init; }
    public required string LogMessage { get; init; }
    public int? ShiftOrderId { get; init; } = null;
}

public class SubmitLogCommandHandler(
    IApplicationDbContext context,
    TimeProvider timeProvider,
    IUserService userService
) : IRequestHandler<SubmitLogCommand>
{
    public async Task Handle(
        SubmitLogCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
           
            var currentUser = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);

            var scanTime = timeProvider.GetLocalNow();

            var log = new ScanLog
            {
                LocationId = request.LocationId,
                UserId = request.UserId,
                ShiftOrderId = request.ShiftOrderId ?? 0, 
                ScanTime = scanTime,
                ScanCode = request.ScanCode,
                Log = request.LogMessage
            };

            context.ScanLogs.Add(log);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            var errorLog = new ScanLog
            {
                LocationId = request.LocationId,
                UserId = request.UserId,
                ShiftOrderId = 0, // Use 0 for error logs
                ScanTime = timeProvider.GetLocalNow(),
                ScanCode = "Error",
                Log = $"Failed to save log: {ex.Message}"
            };

            context.ScanLogs.Add(errorLog);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}