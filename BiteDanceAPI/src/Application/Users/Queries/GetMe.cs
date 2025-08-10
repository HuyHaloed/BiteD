using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Users.Queries;

[Authorize]
public record GetMeQuery(bool IncludeAssignedLocations = false) : IRequest<UserDto>;

public class GetMeQueryHandler(IUserService userService, IMapper mapper)
    : IRequestHandler<GetMeQuery, UserDto>
{
    public async Task<UserDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await userService.GetFromDatabaseOrCreateAsync(
            cancellationToken,
            request.IncludeAssignedLocations
        );

        return mapper.Map<User, UserDto>(user);
    }
}
