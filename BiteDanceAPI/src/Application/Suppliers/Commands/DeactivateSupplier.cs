using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Suppliers.Commands;

[Authorize(RequireSuperAdmin = true)]
public record DeactivateSupplierCommand(int Id) : IRequest;

public class DeactivateSupplierCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeactivateSupplierCommand>
{
    public async Task Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await context.Suppliers.FindOrNotFoundExceptionAsync(
            request.Id,
            cancellationToken
        );

        supplier.IsActive = false;

        await context.SaveChangesAsync(cancellationToken);
    }
}
