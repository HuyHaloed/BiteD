using BiteDanceAPI.Application.Common.Interfaces;

namespace BiteDanceAPI.Application.Locations.Queries;

public record GetLocationsQuery : IRequest<List<LocationDto>>;

public class GetLocationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetLocationsQuery, List<LocationDto>>
{
    public async Task<List<LocationDto>> Handle(
        GetLocationsQuery request,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Locations.ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
