using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Suppliers.Queries;

[Authorize(RequireSupplier = true)]
public record GetUserSupplierQuery(string UserId) : IRequest<SupplierDto>;

public class GetUserSupplierQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetUserSupplierQuery, SupplierDto>
{
    public async Task<SupplierDto> Handle(
        GetUserSupplierQuery request,
        CancellationToken cancellationToken
    )
    {      
         var supplier = await context
            .Suppliers.Include(s => s.AssignedLocations)
            .Where(c => c.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        return mapper.Map<SupplierDto>(supplier);

    }
}



