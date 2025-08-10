using BiteDanceAPI.Application.Departments.Queries;

namespace BiteDanceAPI.Web.Endpoints;

public class Departments : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).MapGet(GetDepartments);
    }

    private async Task<List<DepartmentDto>> GetDepartments(ISender sender)
    {
        return await sender.Send(new GetDepartmentsQuery());
    }
}
