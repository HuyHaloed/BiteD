using BiteDanceAPI.Application.Reports.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class Reports : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetDailyOrdersReport, "order/dailyReport")
            .MapGet(GetShiftCheckinsReport, "checkin/dailyReport");
    }

    public async Task<DailyOrderReportDto> GetDailyOrdersReport(
        ISender sender,
        [FromQuery] DateOnly reportDate,
        [FromQuery] int locationId
    )
    {
        return await sender.Send(new GetDailyOrdersReportQuery(reportDate, locationId));
    }
    
    public async Task<ShiftCheckinsReportDto> GetShiftCheckinsReport(
        ISender sender,
        [FromQuery] DateOnly reportDate,
        [FromQuery] int locationId

    )
    {
        return await sender.Send(new GetShiftCheckinsReportQuery(reportDate,locationId));
    }
}
