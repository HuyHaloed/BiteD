using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Events;

namespace BiteDanceAPI.Application.RedCodes.EventHandlers;

public class RedCodeRequestSubmittedEventHandler(
    IApplicationDbContext context,
    IEmailService emailService
) : INotificationHandler<RedCodeRequestSubmittedEvent>
{
    public async Task Handle(
        RedCodeRequestSubmittedEvent notification,
        CancellationToken cancellationToken
    )
    {
        var location = await context
            .Locations.Include(l => l.Admins)
            .FindOrNotFoundExceptionAsync(
                notification.RedCodeRequest.WorkLocation.Id,
                cancellationToken
            );

        var adminEmails = location.Admins.Select(a => a.Email).ToList();
        if (adminEmails.Count == 0)
        {
            return;
        }

        var emailMessage = new EmailMsg
        {
            To = adminEmails,
            Subject = "New Red Code Request",
            Body =
                $"A new red code request has been submitted by {notification.RedCodeRequest.FullName} ({notification.RedCodeRequest.WorkEmail}) for location {location.Name}."
        };

        await emailService.SendEmailAsync(emailMessage);
    }
}
