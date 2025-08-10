using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Entities;
using BiteDanceAPI.Domain.Enums;

namespace BiteDanceAPI.Application.RedCodes.Command;

[Authorize(RequireAdmin = true)]
public record ApproveRedCodeRequestCommand : IRequest
{
    public required int Id { get; init; }
    public string? Note { get; init; }
}

public class ApproveRedCodeRequestCommandValidator : AbstractValidator<ApproveRedCodeRequestCommand>
{
    public ApproveRedCodeRequestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class ApproveRedCodeRequestCommandHandler(
    IApplicationDbContext context,
    IUserService userService,
    TimeProvider timeProvider
) : IRequestHandler<ApproveRedCodeRequestCommand>
{
    public async Task Handle(
        ApproveRedCodeRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        var admin = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        var redCodeRequest = await context.RedCodeRequests.FindOrNotFoundExceptionAsync(
            request.Id,
            cancellationToken
        );
        admin.AuthorizeAdminOrThrow(redCodeRequest.WorkLocation.Id);

        if (redCodeRequest.Status != RedCodeRequestStatus.Submitted)
        {
            throw new InvalidOperationException("Request status invalid");
        }

        if (redCodeRequest.checkInDate < DateOnly.FromDateTime(timeProvider.GetLocalNow().Date)) // TODO: timing
        {
            throw new InvalidOperationException("Request order has been expired");
        }

        if (redCodeRequest.checkInDate is null) // TODO: timing
        {
            throw new InvalidOperationException("Checkin date is not valid");
        }


        /*
        if (
            redCodeRequest.Role is RedCodeRequesterRole.Contractors or RedCodeRequesterRole.Temps
            && redCodeRequest.ContractEndDate is null
        )
        {
            throw new InvalidOperationException(
                "Contract end date must present for temps and contractors"
            );
        }*/

        var validFrom = new DateTimeOffset(redCodeRequest.checkInDate.Value.ToDateTime(TimeOnly.MinValue));//timeProvider.GetLocalNow();
        var validTill = new DateTimeOffset(
                redCodeRequest.checkInDate.Value.ToDateTime(TimeOnly.MaxValue)
            );
        /*
        if (redCodeRequest.Role == RedCodeRequesterRole.Guests)
        {
            ArgumentNullException.ThrowIfNull(redCodeRequest.GuestDateOfArrival);
            validFrom = new DateTimeOffset(
                redCodeRequest.GuestDateOfArrival.Value.ToDateTime(TimeOnly.MinValue)
            );
            validTill = new DateTimeOffset(
                redCodeRequest.GuestDateOfArrival.Value.ToDateTime(TimeOnly.MaxValue)
            );
        }*/
        /*
        if (redCodeRequest.Role is RedCodeRequesterRole.Contractors or RedCodeRequesterRole.Temps)
        {
            ArgumentNullException.ThrowIfNull(redCodeRequest.ContractEndDate);
            validTill = new DateTimeOffset(
                redCodeRequest.ContractEndDate.Value.ToDateTime(TimeOnly.MaxValue)
            );
        }*/

        var scanCode = new RedScanCode
        {
            RedCodeId = string.Empty.GenerateRandomString(10),
            ValidFrom = validFrom,
            ValidTill = validTill,
            RedCodeRequest = redCodeRequest,
            MaxNumScans = redCodeRequest.OrderNumbers,
            Location = redCodeRequest.WorkLocation,
            LocationId = redCodeRequest.WorkLocation.Id
        };

        
        Console.WriteLine(redCodeRequest);


        scanCode.SetType(CodeType.Red);
        redCodeRequest.Approve(scanCode, admin, request.Note);

        context.RedScanCodes.Add(scanCode);
        await context.SaveChangesAsync(cancellationToken);
    }
}
