using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Application.Common.Security;
using BiteDanceAPI.Domain.Events;

namespace BiteDanceAPI.Application.MonthlyMenus.Commands;

[Authorize(RequireAdmin = true)]
public record PublishMonthlyMenuCommand(int Id) : IRequest;

public class PublishMonthlyMenuCommandHandler(
    IApplicationDbContext context,
    IUserService userService
) : IRequestHandler<PublishMonthlyMenuCommand>
{
    public async Task Handle(PublishMonthlyMenuCommand request, CancellationToken cancellationToken)
    {
        var monthlyMenu = await context
            .MonthlyMenus.Include(m => m.Location)
            .Include(m => m.DailyMenus)
            .ThenInclude(d => d.ShiftMenus)
            .ThenInclude(s => s.Dishes)
            .FindOrNotFoundExceptionAsync(request.Id, cancellationToken);

        var admin = await userService.GetFromDatabaseOrCreateAsync(cancellationToken, true);
        admin.AuthorizeAdminOrThrow(monthlyMenu.LocationId);

        monthlyMenu.IsPublished = true;

        var domainEvent = new MonthlyMenuPublishedEvent(monthlyMenu);
        monthlyMenu.AddDomainEvent(domainEvent);

        await context.SaveChangesAsync(cancellationToken);
    }
}
