using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Users.Commands;

[Authorize(RequireSuperAdmin = true)]
public record UpdateAdminCommand : IRequest
{
    public required string Email { get; init; }
    public IReadOnlyCollection<int> LocationIds { get; init; } = [];
}

public class UpdateAdminCommandValidator : AbstractValidator<UpdateAdminCommand>
{
    public UpdateAdminCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
    }
}

public class UpdateAdminCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateAdminCommand>
{
    public async Task Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(
            x => x.Email == request.Email,
            cancellationToken
        );

        Guard.Against.NotFound(request.Email, user);

        if (!user.IsAdmin)
        {
            throw new InvalidOperationException("User is not an admin");
        }

        user.AssignedLocations.Clear();

        if (request.LocationIds.Count != 0)
        {
            var locations = await context
                .Locations.Where(l => request.LocationIds.Contains(l.Id))
                .ToListAsync(cancellationToken);

            if (locations.Any(x => !x.IsActive))
            {
                throw new InvalidOperationException("Some location is not active");
            }

            user.AssignedLocations = locations;
        }
        else
        {
            user.IsAdmin = false;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
