using BiteDanceAPI.Application.Common.Behaviours;
using BiteDanceAPI.Application.Locations.Queries;
using BiteDanceAPI.Domain.Constants;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Users.Queries;

public class UserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsAdmin { get; init; }
    public bool IsSuperAdmin
    {
        get => Email.Equals(AuthorizationConst.SuperAdminEmail, StringComparison.OrdinalIgnoreCase);
    }
    public bool IsVegetarian { get; init; }
    public bool IsManager { get; init; }

    public bool IsSupplier { get; init; }

    public bool AllowNotifications { get; init; }
    public int? PreferredLocationId { get; init; }

    public IReadOnlyCollection<LocationDto> AssignedLocations { get; init; } =
        new List<LocationDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, UserDto>();
        }
    }
}
