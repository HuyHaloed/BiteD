using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Locations.Commands;

[Authorize(RequireSuperAdmin = true)]
public record ActivateLocationCommand(int Id) : IRequest;

public class ActivateLocationCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ActivateLocationCommand>
{
    public async Task Handle(ActivateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await context.Locations.FindOrNotFoundExceptionAsync(
            request.Id,
            cancellationToken
        );
        location.IsActive = true;

        await context.SaveChangesAsync(cancellationToken);
    }
}
