
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.MonthlyMenus.Queries;
using BiteDanceAPI.Application.Orders.Commands;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;
using System.Text.Json;

namespace BiteDanceAPI.Application.Reports.Queries;

[Authorize]
public record GetShiftCheckinsReportQuery(DateOnly reportDate,int LocationId) : IRequest<ShiftCheckinsReportDto>;

public class GetShiftCheckinsReportQueryHandler(
    IApplicationDbContext context
) : IRequestHandler<GetShiftCheckinsReportQuery, ShiftCheckinsReportDto>
{
    public async Task<ShiftCheckinsReportDto> Handle(
        GetShiftCheckinsReportQuery request,
        CancellationToken cancellationToken
    )
    {

        // Fetch all checkins (green, blue, red)
            var allCheckins = await context.Checkins
                .Where(c => c.LocationId == request.LocationId && c.Datetime.Date == request.reportDate.ToDateTime(TimeOnly.MinValue).Date)
                .Include(c => c.User)
                .ToListAsync(cancellationToken);

        return new ShiftCheckinsReportDto
        {
            ShiftCheckins = allCheckins.Select(g => new CheckinDto
            {
                date = g.Datetime.Date.ToString(),
                type = g.Type,
                userId = g.UserId,
                shift = ""
            }
            ).ToList()
        };
    }
}

public class ShiftCheckinsReportDto
{
    public IReadOnlyList<CheckinDto> ShiftCheckins { get; init; } = [];
    public int numberOfGreenScanned
    {
        get => ShiftCheckins.Count(c => c.type == CodeType.Green);
    }
    public int numberOfBlueScanned
    {
        get => ShiftCheckins.Count(c => c.type == CodeType.Blue);
    }
    public int numberOfRedScanned
    {
        get => ShiftCheckins.Count(c => c.type == CodeType.Red);
    }


}

public class CheckinDto
{

    public required string date { get; set; }

    public CodeType type { get; set; }

    public required string shift { get; set; }

    public string? userId { get; set; }

    public string? scanCodeId { get; set; }

    public string? redcheckin_scancodeid { get; set; }
}
