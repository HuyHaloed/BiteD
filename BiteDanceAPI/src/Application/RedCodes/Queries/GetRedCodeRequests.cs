using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Models;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Application.Departments.Queries;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.RedCodes.Queries;

[Authorize]
public record GetRedCodeRequestsQuery : IRequest<PaginatedList<RedCodeRequestDto>>
{
    public string? Email { get; init; }
    public string? Name { get; init; }
    public int? locationId { get; init; }
    public RedCodeRequestStatus? Status { get; init; }
    public DateOnly? reportDate { get; init; }  // 👈 thêm dòng này
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetRedCodeRequestsQueryHandler(
    IApplicationDbContext context,
    IUserService userService,
    IMapper mapper
) : IRequestHandler<GetRedCodeRequestsQuery, PaginatedList<RedCodeRequestDto>>
{
    public async Task<PaginatedList<RedCodeRequestDto>> Handle(
        GetRedCodeRequestsQuery request,
        CancellationToken cancellationToken
    )
    {
        var admin = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        var query = context
            .RedCodeRequests.Include(r => r.RedScanCode)
            .Include(r => r.Department)
            .Include(r => r.DepartmentChargeCode)
            .Where(r => admin.AssignedLocations.Contains(r.WorkLocation))
            .AsQueryable();

        if (request.reportDate.HasValue)
        {
            var ReportDate = DateOnly.FromDateTime(request.reportDate.Value.ToDateTime(TimeOnly.MinValue).Date);
            query = query.Where(r => r.checkInDate.HasValue &&
                r.checkInDate.Value == ReportDate);
        }


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

         if (request.locationId.HasValue)
        {
            query = query.Where(r => r.WorkLocationId == request.locationId);
        }

        var result = await query
            .OrderByDescending(r => r.Created)
            .ProjectTo<RedCodeRequestDto>(mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}
