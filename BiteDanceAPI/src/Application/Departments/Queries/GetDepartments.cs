using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Departments.Queries;

public record GetDepartmentsQuery : IRequest<List<DepartmentDto>>;

public class GetDepartmentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetDepartmentsQuery, List<DepartmentDto>>
{
    public async Task<List<DepartmentDto>> Handle(
        GetDepartmentsQuery request,
        CancellationToken cancellationToken
    )
    {
        return await context
            .Departments.Include(d => d.ChargeCodes)
            .ProjectTo<DepartmentDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}

public class DepartmentDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IReadOnlyCollection<DepartmentChargeCodeDto> ChargeCodes { get; init; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Department, DepartmentDto>();
            CreateMap<DepartmentChargeCode, DepartmentChargeCodeDto>();
        }
    }
}

public class DepartmentChargeCodeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public string? Code { get; init; }
}
