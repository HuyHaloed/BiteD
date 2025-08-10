using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.RedCodes.Command;

[Authorize(RequireAdmin = true)]
public record DisableRedCodeCommand : IRequest
{
    public required int RedCodeRequestId { get; init; }
    public required string Reason { get; init; }
}

public class DisableRedCodeCommandValidator : AbstractValidator<DisableRedCodeCommand>
{
    public DisableRedCodeCommandValidator()
    {
        RuleFor(x => x.RedCodeRequestId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
    }
}

public class DisableRedCodeCommandHandler(IApplicationDbContext context, IUserService userService)
    : IRequestHandler<DisableRedCodeCommand>
{
    public async Task Handle(DisableRedCodeCommand request, CancellationToken cancellationToken)
    {
        var admin = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        var redCodeRequest = await context
            .RedCodeRequests.Include(r => r.RedScanCode)
            .FindOrNotFoundExceptionAsync(request.RedCodeRequestId, cancellationToken);

        if (!admin.AssignedLocations.Select(l => l.Id).Contains(redCodeRequest.WorkLocation.Id))
        {
            throw new InvalidOperationException(
                "Admin don't have permission to manage this location"
            );
        }

        if (redCodeRequest.Status != RedCodeRequestStatus.Approved)
        {
            throw new InvalidOperationException(
                "Red code request must be in approved status to disable."
            );
        }

        if (redCodeRequest.RedScanCode is { IsDisabled: true })
        {
            throw new InvalidOperationException("Code is already disabled");
        }

        redCodeRequest.Disable(admin);
        redCodeRequest.RedScanCode?.Disable(request.Reason);

        await context.SaveChangesAsync(cancellationToken);
    }
}
