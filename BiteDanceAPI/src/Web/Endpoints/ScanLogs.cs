using BiteDanceAPI.Application.ScanLogs.Commands;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class Logs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(SubmitLog, "LogScanCode");
    }

    private async Task<IResult> SubmitLog(
        ISender sender,
        [FromBody] SubmitLogCommand command
    )
    {
        await sender.Send(command);
        return Results.Empty;
    }
    
}
