using BiteDanceAPI.Domain.Constants;

namespace BiteDanceAPI.Application.Suppliers.Commands;

public class SupplierValidator : AbstractValidator<ISupplierCommand>
{
    public SupplierValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Country)
            .NotEmpty()
            .Must(x => LocationConst.CountryAllowList.Contains(x))
            .WithMessage("Country not in allowlist.");
        RuleFor(x => x.CertificateOfBusinessNumber).NotEmpty();
        RuleFor(x => x.ContractStartDate).NotEmpty();
        RuleFor(x => x.ContractEndDate).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email not valid");
        RuleFor(x => x.PicName).NotEmpty();
        RuleFor(x => x.PicPhoneNumber).NotEmpty();
        RuleFor(x => x.BaseLocation)
            .NotEmpty()
            .Must(x => LocationConst.CityAllowList.Contains(x))
            .WithMessage("BaseLocation not in allowlist");

        RuleFor(x => x)
            .Must(x => x.ContractStartDate < x.ContractEndDate)
            .WithMessage("ContractStartDate must be less than ContractEndDate");
    }
}
