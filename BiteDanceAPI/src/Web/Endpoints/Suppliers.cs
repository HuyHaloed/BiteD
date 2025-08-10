using BiteDanceAPI.Application.Suppliers.Commands;
using BiteDanceAPI.Application.Suppliers.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class Suppliers : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetSuppliers)
            .MapGet(GetSupplier, "{id}")
            .MapGet(GetUserSupplier, "SupplierMapping/{UserId}")
            .MapPost(ActivateSupplier, "{id}/activate")
            .MapPost(DeactivateSupplier, "{id}/deactivate")
            .MapPost(CreateSupplier)
            .MapPut(UpdateSupplier, "{id}");
    }

    private async Task<List<SupplierDto>> GetSuppliers(ISender sender)
    {
        return await sender.Send(new GetSuppliersQuery());
    }

    private async Task<SupplierDto> GetSupplier(ISender sender, int id)
    {
        return await sender.Send(new GetSupplierQuery(id));
    }

    private async Task<SupplierDto> GetUserSupplier(ISender sender, string UserId)
    {
        return await sender.Send(new GetUserSupplierQuery(UserId));
    }

    private async Task<IResult> ActivateSupplier(ISender sender, int id)
    {
        await sender.Send(new ActivateSupplierCommand(id));
        return Results.NoContent();
    }

    private async Task<IResult> DeactivateSupplier(ISender sender, int id)
    {
        await sender.Send(new DeactivateSupplierCommand(id));
        return Results.NoContent();
    }

    private async Task<int> CreateSupplier(ISender sender, [FromBody] CreateSupplierCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<IResult> UpdateSupplier(
        ISender sender,
        int id,
        [FromBody] UpdateSupplierCommand command
    )
    {
        if (id != command.Id)
            return Results.BadRequest();
        await sender.Send(command);

        return Results.NoContent();
    }
}
