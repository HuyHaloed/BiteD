using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Suppliers.Queries;

[Authorize]
public record GetSuppliersQuery : IRequest<List<SupplierDto>>;

public class GetSuppliersQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetSuppliersQuery, List<SupplierDto>>
{
    public async Task<List<SupplierDto>> Handle(
        GetSuppliersQuery request,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Suppliers.ProjectTo<SupplierDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
