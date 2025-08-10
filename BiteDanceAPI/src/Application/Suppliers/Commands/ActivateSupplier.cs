using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Suppliers.Commands;

[Authorize(RequireSuperAdmin = true)]
public record ActivateSupplierCommand(int Id) : IRequest;

public class ActivateSupplierCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ActivateSupplierCommand>
{
    public async Task Handle(ActivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await context.Suppliers.FindOrNotFoundExceptionAsync(
            request.Id,
            cancellationToken
        );

        supplier.IsActive = true;

        await context.SaveChangesAsync(cancellationToken);
    }
}
