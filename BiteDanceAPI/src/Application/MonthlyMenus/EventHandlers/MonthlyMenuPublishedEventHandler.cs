using System.Text;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Events;

namespace BiteDanceAPI.Application.MonthlyMenus.EventHandlers;

public class MonthlyMenuPublishedEventHandler(
    IApplicationDbContext context,
    IEmailService emailService
) : INotificationHandler<MonthlyMenuPublishedEvent>
{
    public async Task Handle(
        MonthlyMenuPublishedEvent notification,
        CancellationToken cancellationToken
    )
    {
        var usersToNotify = await context
            .Users.Where(u =>
                u.PreferredLocationId == notification.MonthlyMenu.LocationId // TODO: index
                && u.AllowNotifications
            )
            .Select(u => u.Email)
            .ToListAsync(cancellationToken);

        var location = await context
            .Locations.Include(l => l.Admins)
            .FindOrNotFoundExceptionAsync(notification.MonthlyMenu.LocationId, cancellationToken);

        var adminsToNotify = location.Admins.Select(a => a.Email).ToList();

        // var groupedMenus = notification
        //     .MonthlyMenu.DailyMenus.SelectMany(d => d.ShiftMenus)
        //     .GroupBy(s => s.Shift)
        //     .Select(g => new { ShiftType = g.Key, Menus = g.ToList() });

        // var emailBody = new StringBuilder();
        // emailBody.AppendLine("The monthly menu has been published. Here are the details:");

        // foreach (var group in groupedMenus)
        // {
        //     emailBody.AppendLine($"Shift: {group.ShiftType}");
        //     foreach (var menu in group.Menus)
        //     {
        //         emailBody.AppendLine($"Date: {menu.DailyMenu.Date}");
        //         emailBody.AppendLine("Dishes:");
        //         foreach (var dish in menu.Dishes)
        //         {
        //             emailBody.AppendLine($"- {dish.Name}");
        //         }
        //     }
        // }
        
        var emailMessage = new EmailMsg
        {
            To = usersToNotify,
            Cc = adminsToNotify,
            Subject = "Monthly Menu Published",
            Body =
                $"The monthly menu for {notification.MonthlyMenu.Location.Name} has been published. Please check the menu in the app."
        };

        await emailService.SendEmailAsync(emailMessage);
    }
}
