namespace BiteDanceAPI.Domain.Entities;

public class Department : BaseEntity
{
    public required string Name { get; set; }

    // Children
    public ICollection<DepartmentChargeCode> ChargeCodes { get; set; } =
        new List<DepartmentChargeCode>();
}
