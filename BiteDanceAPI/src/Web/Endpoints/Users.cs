using BiteDanceAPI.Application.Users.Commands;
using BiteDanceAPI.Application.Users.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetMe, "me")
            .MapPut(UpdateProfile, "me")
            .MapGet(GetUser, "{email}")
            .MapGet(GetAdmins, "admins")
            .MapPut(UpdateAdmin, "admins/{email}")
            .MapPost(DeactivateAdmin, "admins/{email}/deactivate");
    }

    private async Task<UserDto> GetMe(
        ISender sender,
        [FromQuery] bool includeAssignedLocations = false
    )
    {
        return await sender.Send(
            new GetMeQuery() { IncludeAssignedLocations = includeAssignedLocations }
        );
    }

    private async Task<IResult> UpdateProfile(
        ISender sender,
        [FromBody] UpdateProfileCommand command
    )
    {
        await sender.Send(command);

        return Results.Empty;
    }

    private async Task<UserDto> GetUser(ISender sender, string email)
    {
        return await sender.Send(new GetUserQuery(email));
    }


    private async Task<List<AdminDto>> GetAdmins(ISender sender)
    {
        return await sender.Send(new GetAdminsQuery());
    }

    private async Task<IResult> UpdateAdmin(
        ISender sender,
        string email,
        [FromBody] UpdateAdminCommand command
    )
    {
        if (email != command.Email)
        {
            return Results.BadRequest();
        }
        await sender.Send(command);

        return Results.Empty;
    }

    private async Task<IResult> DeactivateAdmin(ISender sender, string email)
    {
        await sender.Send(new DeactivateAdminCommand(email));

        return Results.NoContent();
    }
}
