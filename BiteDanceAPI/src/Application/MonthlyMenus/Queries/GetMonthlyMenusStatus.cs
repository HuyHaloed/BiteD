using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.MonthlyMenus.Queries;

[Authorize(RequireAdmin = true)]
public record GetMonthlyMenusStatusQuery(int LocationId, int Year)
    : IRequest<List<MonthlyMenuStatusDto>>;

public enum MenuStatus
{
    NoMenu,
    Published,
    Uploaded
}

public class GetMonthlyMenusStatusQueryHandler(
    IApplicationDbContext context,
    IUserService userService
) : IRequestHandler<GetMonthlyMenusStatusQuery, List<MonthlyMenuStatusDto>>
{
    public async Task<List<MonthlyMenuStatusDto>> Handle(
        GetMonthlyMenusStatusQuery request,
        CancellationToken cancellationToken
    )
    {
        var admin = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        admin.AuthorizeAdminOrThrow(request.LocationId);

        var monthlyMenus = await context
            .MonthlyMenus.Where(m => m.LocationId == request.LocationId && m.Year == request.Year)
            .ToListAsync(cancellationToken);

        var statuses = new List<MonthlyMenuStatusDto>();
        for (int month = 1; month <= 12; month++)
        {
            var menu = monthlyMenus.FirstOrDefault(m => m.Month == month);
            var status =
                menu == null
                    ? MenuStatus.NoMenu
                    : menu.IsPublished
                        ? MenuStatus.Published
                        : MenuStatus.Uploaded;
            statuses.Add(
                new MonthlyMenuStatusDto
                {
                    Month = month,
                    Status = status,
                    MonthlyMenuId = menu?.Id
                }
            );
        }

        return statuses;
    }
}

public class MonthlyMenuStatusDto
{
    public int Month { get; set; }
    public MenuStatus Status { get; set; }
    public int? MonthlyMenuId { get; set; }
}
