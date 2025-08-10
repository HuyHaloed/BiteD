using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.RedCodes.Queries;

[Authorize(RequireAdmin = true)]
public record GetRedCodeRequestSummaryQuery(int LocationId) : IRequest<RedCodeRequestSummaryDto>;

public class GetRedCodeRequestSummaryQueryHandler(
    IApplicationDbContext context,
    IUserService userService
) : IRequestHandler<GetRedCodeRequestSummaryQuery, RedCodeRequestSummaryDto>
{
    public async Task<RedCodeRequestSummaryDto> Handle(
        GetRedCodeRequestSummaryQuery request,
        CancellationToken cancellationToken
    )
    {
        var admin = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        admin.AuthorizeAdminOrThrow(request.LocationId);

        var totalRequests = await context.RedCodeRequests.CountAsync(
            r => r.WorkLocation.Id == request.LocationId,
            cancellationToken
        );

        var requestsByStatus = await context
            .RedCodeRequests.Where(r => r.WorkLocation.Id == request.LocationId)
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return new RedCodeRequestSummaryDto
        {
            LocationId = request.LocationId,
            TotalRequests = totalRequests,
            RequestsByStatus = requestsByStatus.ToDictionary(x => x.Status, x => x.Count)
        };
    }
}

public class RedCodeRequestSummaryDto
{
    public int LocationId { get; init; }
    public int TotalRequests { get; init; }
    public Dictionary<RedCodeRequestStatus, int> RequestsByStatus { get; init; } = new();
}
