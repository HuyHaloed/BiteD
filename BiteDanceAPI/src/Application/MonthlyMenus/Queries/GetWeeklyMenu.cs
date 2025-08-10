using BiteDanceAPI.Application.Common.Interfaces;

namespace BiteDanceAPI.Application.MonthlyMenus.Queries;

public record GetWeeklyMenuQuery(int LocationId, DateOnly FirstDayOfWeek)
    : IRequest<List<DailyMenuDto>>;

public class GetWeeklyMenuQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetWeeklyMenuQuery, List<DailyMenuDto>>
{
    public async Task<List<DailyMenuDto>> Handle(
        GetWeeklyMenuQuery request,
        CancellationToken cancellationToken
    )
    {
        var startDate = request.FirstDayOfWeek;
        var endDate = startDate.AddDays(6);

        var dailyMenus = await context
            .DailyMenus.Include(d => d.MonthlyMenu)
            .Include(d => d.ShiftMenus)
            .ThenInclude(s => s.Dishes)
            .Where(d =>
                d.MonthlyMenu.LocationId == request.LocationId
                && d.MonthlyMenu.IsPublished
                && d.Date >= startDate
                && d.Date <= endDate
            )
            .ToListAsync(cancellationToken);

        return mapper.Map<List<DailyMenuDto>>(dailyMenus);
    }
}
