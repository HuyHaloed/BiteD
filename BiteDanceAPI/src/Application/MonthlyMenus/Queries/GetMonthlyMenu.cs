using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.MonthlyMenus.Queries;

public record GetMonthlyMenuQuery(int LocationId, int Year, int Month) : IRequest<MonthlyMenuDto?>;

public class GetMonthlyMenuQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetMonthlyMenuQuery, MonthlyMenuDto?>
{
    public async Task<MonthlyMenuDto?> Handle(
        GetMonthlyMenuQuery request,
        CancellationToken cancellationToken
    )
    {
        var monthlyMenu = await context
            .MonthlyMenus.Include(m => m.DailyMenus)
            .ThenInclude(d => d.ShiftMenus)
            .ThenInclude(s => s.Dishes)
            .FirstOrDefaultAsync(
                m =>
                    m.LocationId == request.LocationId
                    && m.Year == request.Year
                    && m.Month == request.Month,
                cancellationToken
            );

        return monthlyMenu is null ? null : mapper.Map<MonthlyMenu, MonthlyMenuDto>(monthlyMenu);
    }
}

public class MonthlyMenuDto
{
    public int Id { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }

    public IReadOnlyCollection<DailyMenuDto> DailyMenus { get; init; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<MonthlyMenu, MonthlyMenuDto>();
        }
    }
}

public class DailyMenuDto
{
    public int MonthlyMenuId { get; init; }
    public DateOnly Date { get; init; }

    public IReadOnlyCollection<ShiftMenuDto> ShiftMenus { get; init; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DailyMenu, DailyMenuDto>();
        }
    }
}

public class ShiftMenuDto
{
    public ShiftType Shift { get; init; }

    public IReadOnlyCollection<DishDto> Dishes { get; init; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ShiftMenu, ShiftMenuDto>();
        }
    }
}

public class DishDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public DishType Type { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Dish, DishDto>();
        }
    }
}
