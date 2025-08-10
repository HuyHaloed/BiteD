using BiteDanceAPI.Application.Orders.Commands;
using BiteDanceAPI.Application.Orders.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BiteDanceAPI.Web.Endpoints;

public class Orders : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetWeeklyOrders)
            .MapGet(GetWeeklyOrdersStatus, "status")
            .MapPost(CreateOrder)
            .MapPost(CancelOrder, "{id}/cancel");
    }

    public async Task<List<DailyOrderStatusDto>> GetWeeklyOrdersStatus(
        ISender sender,
        [FromQuery] int locationId,
        [FromQuery] DateOnly firstDayOfWeek
    )
    {
        return await sender.Send(new GetWeeklyOrderStatusQuery(locationId, firstDayOfWeek));
    }

    public async Task<WeeklyOrderDto> GetWeeklyOrders(
        ISender sender,
        [FromQuery] DateOnly firstDayOfWeek
    )
    {
        return await sender.Send(new GetWeeklyOrderQuery(firstDayOfWeek));
    }

    private async Task<int> CreateOrder(ISender sender, CreateOrderCommand command)
    {
        return await sender.Send(command);
    }

    private async Task<IResult> CancelOrder(ISender sender, int id)
    {
        await sender.Send(new CancelOrderCommand(id));

        return Results.NoContent();
    }
}
