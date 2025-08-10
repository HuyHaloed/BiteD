using BiteDanceAPI.Application.Users.Queries;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.Locations.Queries;

public class LocationDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public LocationType Type { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public bool EnableShift1 { get; init; }
    public bool EnableShift2 { get; init; }
    public bool EnableShift3 { get; init; }
    public bool EnableWeekday { get; init; }
    public bool EnableWeekend { get; init; }
    public bool IsActive { get; init; }

    public int? SupplierId { get; init; }
    public string? SupplierName { get; init; }

    public IReadOnlyCollection<UserDto> Admins { get; init; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Location, LocationDto>();
        }
    }
}
