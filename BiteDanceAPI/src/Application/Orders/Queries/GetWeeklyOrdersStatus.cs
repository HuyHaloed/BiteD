using BiteDanceAPI.Application.Common;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.MonthlyMenus.Queries;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;


namespace BiteDanceAPI.Application.Orders.Queries;

[Authorize(DenySupplier = true)]
public record GetWeeklyOrderStatusQuery(int LocationId, DateOnly FirstDayOfWeek)
    : IRequest<List<DailyOrderStatusDto>>;

public class GetWeeklyOrderStatusQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser,
    IMapper mapper,
    TimeProvider timeProvider
) : IRequestHandler<GetWeeklyOrderStatusQuery, List<DailyOrderStatusDto>>
{
    public async Task<List<DailyOrderStatusDto>> Handle(
        GetWeeklyOrderStatusQuery request,
        CancellationToken cancellationToken
    )
    {
        
        var startDate = request.FirstDayOfWeek;
        var endDate = startDate.AddDays(6);

        var dailyOrders = await context 
            .DailyOrders.Include(d => d.ShiftOrders)
            .ThenInclude(s => s.Dishes)
            .Where(d =>
                d.LocationId == request.LocationId
                && d.UserId == currentUser.Id
                && d.Date >= startDate
                && d.Date <= endDate
                && d.Status == DailyOrderStatus.Ordered
            )
            .ToListAsync(cancellationToken);

        var dailyOrderStatuses = Enumerable
            .Range(0, 7)
            .Select(offset =>
            {
                var currentDate = startDate.AddDays(offset);
                
                var dailyOrder = dailyOrders.FirstOrDefault(d => d.Date == currentDate);

                return new DailyOrderStatusDto
                {
                    Date = currentDate,
                    IsOrdered = dailyOrder is { Status: DailyOrderStatus.Ordered },
                    CanOrder = OrderTimeValidator.IsValidOrderTime(
                        currentDate,
                        
                        timeProvider.GetUtcNow()
                    ),
                    DailyOrderInfo = mapper.Map<DailyOrderDto>(dailyOrder)
                };
            })
            .ToList();

        return dailyOrderStatuses;
    }
}

public class DailyOrderStatusDto
{
    public DateOnly Date { get; set; }
    public bool IsOrdered { get; set; }
    public DailyOrderDto? DailyOrderInfo { get; set; }
    public bool CanOrder { get; set; }
}

public class DailyOrderDto
{
    public int Id { get; set; }
    public DailyOrderStatus Status { get; set; }
    public DateOnly Date { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public bool CanCancelOrder { get; set; }

    // Child
    public IReadOnlyList<ShiftOrderDto> ShiftOrders { get; set; } = new List<ShiftOrderDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DailyOrder, DailyOrderDto>()
                .ForMember(
                    dest => dest.CanCancelOrder,
                    opt => opt.MapFrom<CanCancelOrderResolver>()
                );
            ;
        }
    }
}

public class ShiftOrderDto
{
    public int Id { get; set; }
    public ShiftOrderStatus Status { get; set; }
    public ShiftType ShiftType { get; set; }
    public string? LocationName { get; set; } // For GetWeeklyOrders only

    // Child
    public IReadOnlyList<DishDto> Dishes { get; set; } = new List<DishDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ShiftOrder, ShiftOrderDto>();
        }
    }
}

public class CanCancelOrderResolver(TimeProvider timeProvider)
    : IValueResolver<DailyOrder, DailyOrderDto, bool>
{
    
    public bool Resolve(
        DailyOrder source,
        DailyOrderDto destination,
        bool destMember,
        ResolutionContext context
    )
    {  
        return source.CanBeCanceled(timeProvider.GetUtcNow().DateTime );
    }
}
