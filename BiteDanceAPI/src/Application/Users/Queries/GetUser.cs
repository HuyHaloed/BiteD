using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.Users.Queries;

[Authorize]
public record GetUserQuery(string Email) : IRequest<UserDto>;

public class GetUserQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(
            u => u.Email == request.Email,
            cancellationToken
        );

        Guard.Against.NotFound(request.Email, user);

        return mapper.Map<UserDto>(user);
    }
}
