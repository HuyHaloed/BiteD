using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Users.Queries;

[Authorize(RequireSuperAdmin = true)]
public record GetAdminsQuery : IRequest<List<AdminDto>>;

public class GetAdminsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetAdminsQuery, List<AdminDto>>
{
    public async Task<List<AdminDto>> Handle(
        GetAdminsQuery request,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Users.Include(u => u.AssignedLocations)
            .ThenInclude(l => l.Admins)
            .Where(x => x.IsAdmin)
            .ProjectTo<AdminDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
