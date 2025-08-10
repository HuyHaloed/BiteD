using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Events;

namespace BiteDanceAPI.Application.RedCodes.EventHandlers;

public class RedCodeRequestRejectedEventHandler(IEmailService emailService)
    : INotificationHandler<RedCodeRequestRejectedEvent>
{
    public async Task Handle(
        RedCodeRequestRejectedEvent notification,
        CancellationToken cancellationToken
    )
    {
        var emailMessage = new EmailMsg
        {
            To = [notification.RedCodeRequest.WorkEmail],
            Cc = [notification.Approver.Email],
            Subject = "Red Code Request Rejected",
            Body =
                $"Your red code request has been rejected. Reason: {notification.RedCodeRequest.Note}"
        };

        await emailService.SendEmailAsync(emailMessage);
    }
}
