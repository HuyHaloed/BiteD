using BiteDanceAPI.Application.Common.Models;
using BiteDanceAPI.Application.RedCodes.Command;
using BiteDanceAPI.Application.RedCodes.Queries;
using BiteDanceAPI.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class RedCodes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetRedCodeRequests, "requests")
            .MapGet(GetMyRedCodeRequests, "myrequests")
            .MapPost(DisableRedCode, "redcodes/disable")
            .MapGet(GetRedCodeRequestsSummary, "requests/{locationId}/summary")
            .MapPost(RequestRedCode, "requests")
            .MapPost(ApproveRedCodeRequest, "requests/approve")
            .MapPost(RejectRedCodeRequest, "requests/reject");
    }

    private async Task<RedCodeRequestSummaryDto> GetRedCodeRequestsSummary(
        ISender sender,
        int locationId
    )
    {
        return await sender.Send(new GetRedCodeRequestSummaryQuery(locationId));
    }

    private async Task<int> RequestRedCode(ISender sender, [FromBody] RequestRedCodeCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<IResult> ApproveRedCodeRequest(
        ISender sender,
        [FromBody] ApproveRedCodeRequestCommand command
    )
    {
        await sender.Send(command);

        return Results.NoContent();
    }

    private async Task<IResult> RejectRedCodeRequest(
        ISender sender,
        [FromBody] RejectRedCodeRequestCommand command
    )
    {
        await sender.Send(command);

        return Results.NoContent();
    }

    private async Task<PaginatedList<RedCodeRequestDto>> GetRedCodeRequests(
        ISender sender,
        [FromQuery] string? email,
        [FromQuery] string? name,
        [FromQuery] RedCodeRequestStatus? status,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] DateOnly? ReportDate,
        [FromQuery] int? LocationId
    )
    {
        return await sender.Send(
            new GetRedCodeRequestsQuery()
            {
                Email = email,
                Name = name,
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize,
                reportDate = ReportDate,
                locationId = LocationId
            }
        );
    }

     private async Task<PaginatedList<RedCodeRequestDto>> GetMyRedCodeRequests(
        ISender sender,
        [FromQuery] string? email,
        [FromQuery] string? name,
        [FromQuery] RedCodeRequestStatus? status,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize
    )
    {
        return await sender.Send(
            new GetMyRedCodeRequestsQuery()
            {
                Email = email,
                Name = name,
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize
            }
        );
    }

    private async Task DisableRedCode(ISender sender, [FromBody] DisableRedCodeCommand command)
    {
        await sender.Send(command);
    }
}
