using BiteDanceAPI.Application.System.Queries;

namespace BiteDanceAPI.Web.Endpoints;

public class System : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).MapGet(GetConfigs, "configs");
    }

    private async Task<ConfigDto> GetConfigs(ISender sender)
    {
        return await sender.Send(new GetConfigsQuery());
    }
}
