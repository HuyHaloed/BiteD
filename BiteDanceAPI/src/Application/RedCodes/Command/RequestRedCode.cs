using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;
using BiteDanceAPI.Domain.Events;

namespace BiteDanceAPI.Application.RedCodes.Command;

public record RequestRedCodeCommand : IRequest<int>
{
    public required string FullName { get; init; }
    //public required RedCodeRequesterRole Role { get; init; }
    public required string UserId { get; init; }
    public required int WorkLocationId { get; init; }
    public required string WorkEmail { get; init; }
    public required int OrderNumbers { get; init; }
    public DateOnly? checkInDate { get; init; } // Required for temps & contractors
    public required int DepartmentId { get; init; }

    // Required for guests
    public int? DepartmentChargeCodeId { get; init; }
    //public string? GuestTitle { get; init; }
    //public string? GuestContactNumber { get; init; }
    public string? GuestPurposeOfVisit { get; init; }
    //public DateOnly? GuestDateOfArrival { get; init; }
}

public class RequestRedCodeCommandValidator : AbstractValidator<RequestRedCodeCommand>
{
    public RequestRedCodeCommandValidator(IApplicationDbContext context, TimeProvider timeProvider)
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.WorkEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.OrderNumbers).NotEmpty()
        .LessThan(1001).GreaterThan(0).WithMessage("Please enter an order number from 1 to 1000");
        RuleFor(x => x.checkInDate!.Value).NotEmpty()
        .GreaterThan(DateOnly.FromDateTime(
            timeProvider.GetLocalNow().DateTime.AddHours(8)
        ))
    .WithMessage("Request order must be placed before 4pm GMT+7 of the previous day.");
    

        /*
                RuleFor(x => x)
                    .Must(x =>
                        x.Role != RedCodeRequesterRole.Temps && x.Role != RedCodeRequesterRole.Contractors
                        || x.ContractEndDate != default
                    )
                    .WithMessage("Contract end date must be present for Temps or Contractors.");*/

        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.WorkLocationId).NotEmpty();
        RuleFor(x => x.OrderNumbers).NotEmpty();
        /* 
        RuleFor(x => x)
            .Must(x =>
                x.Role != RedCodeRequesterRole.Guests
                || (
                    x.DepartmentChargeCodeId != null
                    && !string.IsNullOrEmpty(x.GuestTitle)
                    && !string.IsNullOrEmpty(x.GuestContactNumber)
                    && !string.IsNullOrEmpty(x.GuestPurposeOfVisit)
                    && x.GuestDateOfArrival != null
                )
            )
            .WithMessage("All guest-specific fields must be provided for Guests role.");*/
    }
}

public class RequestRedCodeCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RequestRedCodeCommand, int>
{
    public async Task<int> Handle(
        RequestRedCodeCommand request,
        CancellationToken cancellationToken
    )
    {
        var workLocation = await context.Locations.FindOrNotFoundExceptionAsync(
            request.WorkLocationId,
            cancellationToken
        );

        if (!workLocation.IsActive)
        {
            throw new InvalidOperationException("The selected work location is not active.");
        }

        var department = await context.Departments.FindOrNotFoundExceptionAsync(
            request.DepartmentId,
            cancellationToken
        );

        var departmentChargeCode =
            request.DepartmentChargeCodeId != null
                ? await context.DepartmentChargeCodes.FindAsync(
                    request.DepartmentChargeCodeId,
                    cancellationToken
                )
                : null;

        if (departmentChargeCode != null && departmentChargeCode.DepartmentId != department.Id)
        {
            throw new InvalidOperationException(
                "The provided department charge code doesn't belong to the department."
            );
        }

        var redCodeRequest = new RedCodeRequest
        {
            FullName = request.FullName,
            //Role = request.Role,
            WorkLocation = workLocation,
            WorkEmail = request.WorkEmail,
            checkInDate = request.checkInDate,
            Note = string.Empty,
            Department = department,
            DepartmentChargeCode = departmentChargeCode,
            OrderNumbers = request.OrderNumbers,
            //GuestTitle = request.GuestTitle,
            //GuestContactNumber = request.GuestContactNumber,
            GuestPurposeOfVisit = request.GuestPurposeOfVisit,
            //GuestDateOfArrival = request.GuestDateOfArrival
        };

        redCodeRequest.AddDomainEvent(new RedCodeRequestSubmittedEvent(redCodeRequest));
        context.RedCodeRequests.Add(redCodeRequest);
        await context.SaveChangesAsync(cancellationToken);

        return redCodeRequest.Id;
    }
}
