using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Locations.Queries;

[Authorize]
public record GetLocationQuery(int Id) : IRequest<LocationDto>; // TODO: test

public class GetLocationQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetLocationQuery, LocationDto>
{
    public async Task<LocationDto> Handle(
        GetLocationQuery request,
        CancellationToken cancellationToken
    )
    {
        var location = await context
            .Locations.Include(l => l.Supplier)
            .Include(l => l.Admins)
            .FindOrNotFoundExceptionAsync(request.Id, cancellationToken);

        return mapper.Map<LocationDto>(location);
    }
}
