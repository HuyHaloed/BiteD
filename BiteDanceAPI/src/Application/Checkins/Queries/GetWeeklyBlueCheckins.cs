using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Constants;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Checkins.Queries;

[Authorize(DenySupplier = true)]
public record GetWeeklyBlueCheckinsQuery(DateOnly FirstDayOfWeek) : IRequest<WeeklyBlueCheckinDto>;

public class GetWeeklyBlueCheckinsQueryHandler(
    IApplicationDbContext context,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<GetWeeklyBlueCheckinsQuery, WeeklyBlueCheckinDto>
{
    public async Task<WeeklyBlueCheckinDto> Handle(
        GetWeeklyBlueCheckinsQuery request,
        CancellationToken cancellationToken
    )
    {
        var startDate = request.FirstDayOfWeek;
        var endDate = startDate.AddDays(6);

        var checkins = await context
            .BlueCheckins.Include(b => b.Location)
            .Where(c =>
                c.UserId == currentUser.Id
                && c.Datetime >= startDate.ToDateTime(TimeOnly.MinValue)
                && c.Datetime <= endDate.ToDateTime(TimeOnly.MaxValue)
            )
            .ToListAsync(cancellationToken);

        return new WeeklyBlueCheckinDto { Checkins = mapper.Map<List<BlueCheckinDto>>(checkins) };
    }
}

public class WeeklyBlueCheckinDto
{
    public int MaxScansPerDay { get; } = BlueCheckinConst.MaxScansPerDay;
    public int MaxScansPerWeek { get; } = BlueCheckinConst.MaxScansPerWeek;
    public int ScansThisWeek
    {
        get => Checkins.Count;
    }
    public int ScansRemaining
    {
        get => MaxScansPerWeek - ScansThisWeek;
    }
    public List<BlueCheckinDto> Checkins { get; init; } = new();
}

public class BlueCheckinDto
{
    public int Id { get; init; }
    public DateTimeOffset Datetime { get; init; }
    public string LocationName { get; init; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BlueCheckin, BlueCheckinDto>();
        }
    }
}
