using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;

namespace BiteDanceAPI.Application.MonthlyMenus.Commands;

[Authorize(RequireAdmin = true)]
public record DeleteMonthlyMenuCommand(int Id) : IRequest;

public class DeleteMonthlyMenuCommandHandler(
    IApplicationDbContext context,
    IUserService userService
) : IRequestHandler<DeleteMonthlyMenuCommand>
{
    public async Task Handle(DeleteMonthlyMenuCommand request, CancellationToken cancellationToken)
    {
        var monthlyMenu = await context.MonthlyMenus.FindOrNotFoundExceptionAsync(
            request.Id,
            cancellationToken
        );

        if (monthlyMenu.IsPublished)
        {
            throw new InvalidOperationException("Cannot delete a published monthly menu.");
        }

        var admin = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        admin.AuthorizeAdminOrThrow(monthlyMenu.LocationId);

        context.MonthlyMenus.Remove(monthlyMenu);
        await context.SaveChangesAsync(cancellationToken);
    }
}
