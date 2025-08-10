using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Enums;
using InvalidOperationException = System.InvalidOperationException;

namespace BiteDanceAPI.Application.RedCodes.Command;

[Authorize(RequireAdmin = true)]
public record RejectRedCodeRequestCommand : IRequest
{
    public required int Id { get; init; }
    public required string Note { get; init; }
}

public class RejectRedCodeRequestCommandValidator : AbstractValidator<RejectRedCodeRequestCommand>
{
    public RejectRedCodeRequestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Note).NotEmpty();
    }
}

public class RejectRedCodeRequestCommandHandler(
    IApplicationDbContext context,
    IUserService userService
) : IRequestHandler<RejectRedCodeRequestCommand>
{
    public async Task Handle(
        RejectRedCodeRequestCommand request,
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

        redCodeRequest.Reject(admin, request.Note);
        await context.SaveChangesAsync(cancellationToken);
    }
}
