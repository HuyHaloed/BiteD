using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.MonthlyMenus.Queries;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Orders.Queries;

[Authorize(DenySupplier = true)]
public record GetWeeklyOrderQuery(DateOnly FirstDayOfWeek) : IRequest<WeeklyOrderDto>;

public class GetWeeklyOrderQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<GetWeeklyOrderQuery, WeeklyOrderDto>
{
    public async Task<WeeklyOrderDto> Handle(
        GetWeeklyOrderQuery request,
        CancellationToken cancellationToken
    )
    {
        var startDate = request.FirstDayOfWeek;
        var endDate = startDate.AddDays(6);

        var dailyOrders = await context
            .DailyOrders.Include(s => s.Location)
            .Include(d => d.ShiftOrders)
            .ThenInclude(s => s.Dishes)
            .Where(d =>
                d.UserId == currentUser.Id
                && d.Date >= startDate
                && d.Date <= endDate
                && d.Status == DailyOrderStatus.Ordered
            )
            .ToListAsync(cancellationToken);

        return new WeeklyOrderDto { DailyOrders = mapper.Map<List<DailyOrderDto>>(dailyOrders) };
    }
}

public class WeeklyOrderDto
{
    public IReadOnlyList<DailyOrderDto> DailyOrders { get; init; } = [];
    public int NumberOfShiftsOrdered
    {
        get => DailyOrders.Sum(d => d.ShiftOrders.Count);
    }
    public int NumberOfShiftsScanned
    {
        get => DailyOrders.Sum(d => d.ShiftOrders.Count(s => s.Status == ShiftOrderStatus.Scanned));
    }
    public int NumberOfShiftsRemaining
    {
        get => NumberOfShiftsOrdered - NumberOfShiftsScanned;
    }
}
