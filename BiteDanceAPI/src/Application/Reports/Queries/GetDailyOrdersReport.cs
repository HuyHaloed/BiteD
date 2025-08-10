
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.MonthlyMenus.Queries;
using BiteDanceAPI.Application.Orders.Commands;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;
using System.Text.Json;

namespace BiteDanceAPI.Application.Reports.Queries;

[Authorize]
public record GetDailyOrdersReportQuery(DateOnly reportDate, int LocationId) : IRequest<DailyOrderReportDto>;

public class GetDailyOrdersReportQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetDailyOrdersReportQuery, DailyOrderReportDto>
{
    public async Task<DailyOrderReportDto> Handle(
        GetDailyOrdersReportQuery request,
        CancellationToken cancellationToken
    )
    {
        var reportDate = request.reportDate;
        var dailyReports = await context
            .DailyOrders.Include(s => s.Location)
            .Include(d => d.ShiftOrders)
            .ThenInclude(s => s.Dishes)
            .Where(d =>
                    d.ShiftOrders.Any(s =>
                        s.DailyOrder.Date == reportDate && s.LocationId == request.LocationId &&
                        (s.Status == ShiftOrderStatus.Ordered || s.Status == ShiftOrderStatus.Scanned)
                    )
            )
            .ToListAsync(cancellationToken);

        var grouped = dailyReports
            .SelectMany(order => order.ShiftOrders, (order, shift) => new
            {
                order.Date,
                order.Location.Name,
                ShiftType = shift.ShiftType.ToString(), // hoặc shift.ShiftName
                Dishes = shift.Dishes
            })
            .SelectMany(x => x.Dishes, (x, dish) => new
            {
                x.Date,
                LocationName = x.Name,
                x.ShiftType,
                DishId = dish.Id,
                DishName = dish.Name
            })
            .GroupBy(x => new { x.ShiftType, x.DishId, x.DishName })
            .Select(g => new dailyReportDish
            {
                date = g.First().Date.ToString("yyyy-MM-dd"),
                dish = g.Key.DishName,
                shift = g.Key.ShiftType,
                ordersNumber = g.Count()
            })
            .ToList();

         

        return new DailyOrderReportDto
        {
            dailyOrders = grouped
        };
    }
}

public class DailyOrderReportDto
{
    public IReadOnlyList<dailyReportDish> dailyOrders { get; init; } = [];
     public int numberOfPreOrder
    {
        get => dailyOrders.Sum(c => c.ordersNumber);
    }

}

public class dailyReportDish
{

    public IReadOnlyList<dailyReportDish> dailyOrders { get; init; } = [];
    public required string date { get; set; }

    public required string dish { get; set; }

    public required string shift { get; set; }

    public required int ordersNumber { get; set; }
}
