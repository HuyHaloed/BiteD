using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Users.Commands;

[Authorize(RequireSuperAdmin = true)]
public record DeactivateAdminCommand(string Email) : IRequest;

public class DeactivateAdminCommandValidator : AbstractValidator<DeactivateAdminCommand>
{
    public DeactivateAdminCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}

public class DeactivateAdminCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeactivateAdminCommand>
{
    public async Task Handle(DeactivateAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await context
            .Users.Include(u => u.AssignedLocations)
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        Guard.Against.NotFound(request.Email, user);

        user.IsAdmin = false;
        user.AssignedLocations.Clear();

        await context.SaveChangesAsync(cancellationToken);
    }
}
