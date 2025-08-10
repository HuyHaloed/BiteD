using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Suppliers.Commands;

[Authorize(RequireSuperAdmin = true)]
public record UpdateSupplierCommand : IRequest, ISupplierCommand
{
    public required int Id { get; init; }
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

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSupplierCommandValidator(IApplicationDbContext context)
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

public class UpdateSupplierCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSupplierCommand>
{
    public async Task Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await context
            .Suppliers.Include(s => s.AssignedLocations)
            .FindOrNotFoundExceptionAsync(request.Id, cancellationToken);

        supplier.Name = request.Name;
        supplier.Country = request.Country;
        supplier.CertificateOfBusinessNumber = request.CertificateOfBusinessNumber;
        supplier.ContractStartDate = request.ContractStartDate;
        supplier.ContractEndDate = request.ContractEndDate;
        supplier.Address = request.Address;
        supplier.PhoneNumber = request.PhoneNumber;
        supplier.Email = request.Email;
        supplier.PicName = request.PicName;
        supplier.PicPhoneNumber = request.PicPhoneNumber;
        supplier.BaseLocation = request.BaseLocation;

        if (request.LocationIds.Count != 0)
        {
            var locations = await context
                .Locations.Where(l => request.LocationIds.Contains(l.Id))
                .ToListAsync(cancellationToken);
            supplier.AssignedLocations = locations;
        }
        else
        {
            supplier.AssignedLocations.Clear();
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
