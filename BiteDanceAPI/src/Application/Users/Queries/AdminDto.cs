using BiteDanceAPI.Application.Locations.Queries;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Users.Queries;

public class AdminDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsAdmin { get; init; }
    public IReadOnlyCollection<LocationDto> AssignedLocations { get; set; } =
        new List<LocationDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, AdminDto>();
        }
    }
}
