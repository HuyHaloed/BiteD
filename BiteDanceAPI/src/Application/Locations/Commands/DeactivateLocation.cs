using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Locations.Commands;

[Authorize(RequireSuperAdmin = true)]
public record DeactivateLocationCommand(int Id) : IRequest;

public class DeactivateLocationCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeactivateLocationCommand>
{
    public async Task Handle(DeactivateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await context.Locations.FindOrNotFoundExceptionAsync(
            request.Id,
            cancellationToken
        );
        location.IsActive = false;

        await context.SaveChangesAsync(cancellationToken);
    }
}
