using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Constants;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Locations.Commands;

[Authorize(RequireSuperAdmin = true)]
public record CreateLocationCommand : IRequest<int>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public LocationType Type { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public bool EnableShift1 { get; init; }
    public bool EnableShift2 { get; init; }
    public bool EnableShift3 { get; init; }
    public bool EnableWeekday { get; init; }
    public bool EnableWeekend { get; init; }
    public int? SupplierId { get; init; }
    public IReadOnlyCollection<string> AdminEmails { get; init; } = [];
}

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateLocationCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");

        RuleFor(x => x.City)
            .Must(x => LocationConst.CityAllowList.Contains(x))
            .WithMessage("City must be in allow list");

        RuleFor(x => x.Country)
            .Must(x => LocationConst.CountryAllowList.Contains(x))
            .WithMessage("Country must be in allow list");

        RuleForEach(x => x.AdminEmails)
            .MustAsync(EmailExists)
            .WithMessage("Admin email does not exist in the database");
    }

    private async Task<bool> EmailExists(string email, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }
}

public class CreateLocationCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateLocationCommand, int>
{
    public async Task<int> Handle(
        CreateLocationCommand request,
        CancellationToken cancellationToken
    )
    {
        Supplier? supplier = null;
        if (request.SupplierId.HasValue)
        {
            supplier = await context.Suppliers.FindOrNotFoundExceptionAsync(
                request.SupplierId.Value,
                cancellationToken
            );
        }

        var admins = await context
            .Users.Where(u => request.AdminEmails.Contains(u.Email))
            .ToListAsync(cancellationToken);

        var location = new Location()
        {
            Name = request.Name,
            City = request.City,
            Country = request.Country,
            Type = request.Type,
            Description = request.Description,
            EnableShift1 = request.EnableShift1,
            EnableShift2 = request.EnableShift2,
            EnableShift3 = request.EnableShift3,
            EnableWeekend = request.EnableWeekend,
            EnableWeekday = request.EnableWeekday,
            IsActive = true,
            Supplier = supplier,
            Admins = admins
        };

        await context.Locations.AddAsync(location, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return location.Id;
    }
}
