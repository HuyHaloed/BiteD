using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Users.Commands;

[Authorize]
public record UpdateProfileCommand : IRequest
{
    public string Name { get; init; } = string.Empty;
    public bool IsVegetarian { get; init; }
    public bool AllowNotifications { get; init; }
    public int? PreferredLocationId { get; init; }
}

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProfileCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").MaximumLength(256);

        RuleFor(x => x.PreferredLocationId)
            .MustAsync(LocationExists)
            .WithMessage("Preferred location does not exist.");
    }

    private async Task<bool> LocationExists(int? locationId, CancellationToken cancellationToken)
    {
        if (locationId == null)
            return true;

        return await _context.Locations.AnyAsync(l => l.Id == locationId, cancellationToken);
    }
}

public class UpdateProfileCommandHandler(IApplicationDbContext context, IUserService userService)
    : IRequestHandler<UpdateProfileCommand>
{
    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userService.GetFromDatabaseOrCreateAsync(cancellationToken);

        user.Name = request.Name;
        user.IsVegetarian = request.IsVegetarian;
        user.AllowNotifications = request.AllowNotifications;
        user.PreferredLocationId = request.PreferredLocationId;

        await context.SaveChangesAsync(cancellationToken);
    }
}
