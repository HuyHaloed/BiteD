using BiteDanceAPI.Application.Locations.Queries;
using BiteDanceAPI.Domain.Entities;

namespace BiteDanceAPI.Application.Suppliers.Queries;

public class SupplierDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string CertificateOfBusinessNumber { get; init; }
    public DateOnly ContractStartDate { get; init; }
    public DateOnly ContractEndDate { get; init; }
    public required string Address { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required string PicName { get; init; }
    public required string PicPhoneNumber { get; init; }
    public required string BaseLocation { get; init; }
    public string? UserId { get; set; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<LocationDto> AssignedLocations { get; init; } =
        new List<LocationDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Supplier, SupplierDto>();
        }
    }
}
