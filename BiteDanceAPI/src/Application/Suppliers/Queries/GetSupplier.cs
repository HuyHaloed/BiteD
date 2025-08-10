using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Suppliers.Queries;

[Authorize]
public record GetSupplierQuery(int Id) : IRequest<SupplierDto>;

public class GetSupplierQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetSupplierQuery, SupplierDto>
{
    public async Task<SupplierDto> Handle(
        GetSupplierQuery request,
        CancellationToken cancellationToken
    )
    {
        var supplier = await context
            .Suppliers.Include(s => s.AssignedLocations)
            .FindOrNotFoundExceptionAsync(request.Id, cancellationToken);

        return mapper.Map<SupplierDto>(supplier);
    }
}
