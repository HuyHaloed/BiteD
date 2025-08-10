namespace BiteDanceAPI.Application.Suppliers.Commands;

public interface ISupplierCommand
{
    string Name { get; }
    string Country { get; }
    string CertificateOfBusinessNumber { get; }
    DateOnly ContractStartDate { get; }
    DateOnly ContractEndDate { get; }
    string Address { get; }
    string PhoneNumber { get; }
    string Email { get; }
    string PicName { get; }
    string PicPhoneNumber { get; }
    string BaseLocation { get; }
}
