using BiteDanceAPI.Application.MonthlyMenus.Commands;
using BiteDanceAPI.Application.MonthlyMenus.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class MonthlyMenus : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetMonthlyMenuTemplate, "template")
            .MapGet(GetMonthlyMenus)
            .MapGet(GetWeeklyMenu, "weekly")
            .MapGet(GetMonthlyMenusStatus, "status")
            .MapPost(CreateMonthlyMenus)
            .MapDelete(DeleteMonthlyMenus, "{id}/delete")
            .MapPost(PublishMonthlyMenu, "{id}/publish");
    }

    public async Task<int> CreateMonthlyMenus(
        ISender sender,
        [FromForm] IFormFile file,
        [FromQuery] int locationId,
        [FromQuery] int year,
        [FromQuery] int month
    )
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var excelData = memoryStream.ToArray();

        var command = MonthlyMenuTemplateParser.ParseTemplate(excelData, locationId, year, month);
        return await sender.Send(command);
    }

    public async Task<IResult> DeleteMonthlyMenus(ISender sender, int id)
    {
        await sender.Send(new DeleteMonthlyMenuCommand(id));

        return Results.NoContent();
    }

    public async Task<IResult> PublishMonthlyMenu(ISender sender, int id)
    {
        await sender.Send(new PublishMonthlyMenuCommand(id));

        return Results.NoContent();
    }

    public async Task<MonthlyMenuDto?> GetMonthlyMenus(
        ISender sender,
        int locationId,
        int year,
        int month
    )
    {
        return await sender.Send(new GetMonthlyMenuQuery(locationId, year, month));
    }

    public async Task<IResult> GetMonthlyMenuTemplate(
        ISender sender,
        [FromQuery] int locationId,
        [FromQuery] int year,
        [FromQuery] int month
    )
    {
        var query = new GetMonthlyMenuTemplateQuery(locationId, year, month);
        var fileContent = await sender.Send(query);

        return Results.File(
            fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "MonthlyMenuTemplate.xlsx"
        );
    }

    public async Task<List<MonthlyMenuStatusDto>> GetMonthlyMenusStatus(
        ISender sender,
        int locationId,
        int year
    )
    {
        return await sender.Send(new GetMonthlyMenusStatusQuery(locationId, year));
    }

    public async Task<List<DailyMenuDto>> GetWeeklyMenu(
        ISender sender,
        [FromQuery] int locationId,
        [FromQuery] DateOnly firstDayOfWeek
    )
    {
        return await sender.Send(new GetWeeklyMenuQuery(locationId, firstDayOfWeek));
    }
}
