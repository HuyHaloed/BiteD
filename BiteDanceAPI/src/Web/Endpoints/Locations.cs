using BiteDanceAPI.Application.Locations.Commands;
using BiteDanceAPI.Application.Locations.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class Locations : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetLocations)
            .MapGet(GetLocation, "{id}")
            .MapPost(CreateLocation)
            .MapPost(ActivateLocation, "{id}/activate")
            .MapPost(DeactivateLocation, "{id}/deactivate")
            .MapPut(UpdateLocation, "{id}");
    }

    private async Task<List<LocationDto>> GetLocations(ISender sender)
    {
        return await sender.Send(new GetLocationsQuery());
    }

    private async Task<LocationDto> GetLocation(ISender sender, int id)
    {
        return await sender.Send(new GetLocationQuery(id));
    }

    private async Task<int> CreateLocation(ISender sender, CreateLocationCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<IResult> ActivateLocation(ISender sender, int id)
    {
        await sender.Send(new ActivateLocationCommand(id));

        return Results.NoContent();
    }

    private async Task<IResult> DeactivateLocation(ISender sender, int id)
    {
        await sender.Send(new DeactivateLocationCommand(id));

        return Results.NoContent();
    }

    private async Task<IResult> UpdateLocation(
        ISender sender,
        int id,
        [FromBody] UpdateLocationCommand command
    )
    {
        if (id != command.Id)
            return Results.BadRequest();
        await sender.Send(command);

        return Results.NoContent();
    }
}
