using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Suppliers.Commands;

[Authorize(RequireSuperAdmin = true)]
public record CreateSupplierCommand : IRequest<int>, ISupplierCommand
{
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string CertificateOfBusinessNumber { get; init; }
    public DateOnly ContractStartDate { get; init; }
    public DateOnly ContractEndDate { get; init; }
    public required string Address { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required string PicName { get; init; }
    public required string PicPhoneNumber { get; init; }
    public required string BaseLocation { get; init; }
    public List<int> LocationIds { get; init; } = [];
}

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateSupplierCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        Include(new SupplierValidator());

        RuleForEach(x => x.LocationIds)
            .MustAsync(LocationExists)
            .WithMessage("Location does not exist.");
    }

    private async Task<bool> LocationExists(int locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations.AnyAsync(l => l.Id == locationId, cancellationToken);
    }
}

public class CreateSupplierCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSupplierCommand, int>
{
    public async Task<int> Handle(
        CreateSupplierCommand request,
        CancellationToken cancellationToken
    )
    {
        var supplier = new Supplier
        {
            Name = request.Name,
            Country = request.Country,
            CertificateOfBusinessNumber = request.CertificateOfBusinessNumber,
            ContractStartDate = request.ContractStartDate,
            ContractEndDate = request.ContractEndDate,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            PicName = request.PicName,
            PicPhoneNumber = request.PicPhoneNumber,
            BaseLocation = request.BaseLocation,
            IsActive = true,
        };

        if (request.LocationIds.Count != 0)
        {
            var locations = await context
                .Locations.Where(l => request.LocationIds.Contains(l.Id))
                .ToListAsync(cancellationToken);
            supplier.AssignedLocations = locations;
        }

        await context.Suppliers.AddAsync(supplier, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}
