namespace BiteDanceAPI.Domain.Entities;

public class DepartmentChargeCode : BaseEntity
{
    public required string Name { get; set; }

    // Parent
    public Department Department { get; set; } = default!;
    public int DepartmentId { get; set; }
    public string? Code { get; set; }
}
