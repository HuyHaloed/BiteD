using BiteDanceAPI.Application.Checkins.Commands;
using BiteDanceAPI.Application.Checkins.Queries;
using BiteDanceAPI.Application.Locations.Commands;
using BiteDanceAPI.Application.Locations.Queries;
using BiteDanceAPI.Application.Orders.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class Checkins : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CheckinBlueQr, "blue")
            .MapPost(CheckinGreenQr, "green")
            .MapPost(CheckinRedQr, "red")
            .MapGet(GetWeeklyBlueCheckins, "blue/weekly")
            .MapGet(ExportFteCheckins, "export"); // .MapPost(CheckinPurpleQr, "purple");
    }

    private async Task<WeeklyBlueCheckinDto> GetWeeklyBlueCheckins(
        ISender sender,
        [FromQuery] DateOnly firstDayOfWeek
    )
    {
        return await sender.Send(new GetWeeklyBlueCheckinsQuery(firstDayOfWeek));
    }

    private async Task<CheckinResult> CheckinBlueQr(ISender sender, CheckinBlueQrCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<CheckinResult> CheckinGreenQr(ISender sender, CheckinGreenQrCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<CheckinResult> CheckinRedQr(ISender sender, CheckinRedQrCommand command)
    {
        return await sender.Send(command);
    }

    // private async Task<IResult> CheckinPurpleQr(ISender sender, CheckinPurpleQrCommand command)
    // {
    //     await sender.Send(command);
    //
    //     return Results.NoContent();
    // }

    private async Task<IResult> ExportFteCheckins(
        ISender sender,
        [FromQuery] int locationId,
        [FromQuery] int year,
        [FromQuery] int month
    )
    {
        var fileContent = await sender.Send(new ExportFteCheckinsCommand(locationId, year, month));

        return Results.File(
            fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "FteCheckins.xlsx"
        );
    }
}
