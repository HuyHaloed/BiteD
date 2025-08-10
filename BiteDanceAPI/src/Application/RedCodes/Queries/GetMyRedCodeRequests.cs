using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Models;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.Departments.Queries;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.RedCodes.Queries;
public record GetMyRedCodeRequestsQuery : IRequest<PaginatedList<RedCodeRequestDto>>
{
    public string? Email { get; init; }
    public string? Name { get; init; }
    public RedCodeRequestStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetMyRedCodeRequestsQueryHandler(
    IApplicationDbContext context,
    IUserService userService,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<GetMyRedCodeRequestsQuery, PaginatedList<RedCodeRequestDto>>
{
    public async Task<PaginatedList<RedCodeRequestDto>> Handle(
        GetMyRedCodeRequestsQuery request,
        CancellationToken cancellationToken
    )
    {
        var admin = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);

        var query = context
            .RedCodeRequests.Include(r => r.RedScanCode)
            .Include(r => r.Department)
            .Include(r => r.DepartmentChargeCode)
            .Where(r => r.CreatedBy == currentUser.Id)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Email))
        {
            // query = query.Where(r => EF.Functions.Like(r.WorkEmail, $"%{request.Email}%"));
            query = query.Where(r => r.WorkEmail.Contains(request.Email));
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            // query = query.Where(r => EF.Functions.Like(r.FullName, $"%{request.Name}%"));
            query = query.Where(r => r.FullName.Contains(request.Name));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status);
        }

        var result = await query
            .OrderByDescending(r => r.Created)
            .ProjectTo<RedCodeRequestDto>(mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}

public class RedCodeRequestDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string WorkEmail { get; init; } = string.Empty;
    public RedCodeRequestStatus Status { get; init; }
    public string Note { get; init; } = string.Empty;
    public string WorkLocationName { get; init; } = string.Empty;
    public int OrderNumbers { get; init; }
    public RedCodeRequesterRole Role { get; init; }
    public DateTimeOffset Created { get; init; }

    public DateOnly? CheckInDate { get; init; }
    public string? GuestTitle { get; init; }
    public string? GuestContactNumber { get; init; }
    public string? GuestPurposeOfVisit { get; init; }
    public DateOnly? GuestDateOfArrival { get; init; }

    public BasicDepartmentDto? Department { get; init; }
    public DepartmentChargeCodeDto? DepartmentChargeCode { get; init; }
    public RedScanCode? RedScanCode { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RedCodeRequest, RedCodeRequestDto>();
            // .ForMember(d => d.WorkLocation, opt => opt.MapFrom(s => s.WorkLocation.Name));
        }
    }
}

public class BasicDepartmentDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Department, BasicDepartmentDto>();
        }
    }
}
