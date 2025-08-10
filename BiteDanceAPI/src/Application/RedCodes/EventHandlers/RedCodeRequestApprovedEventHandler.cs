using System.Drawing;
using BiteDanceAPI.Application.Common.Interfaces;
using BiteDanceAPI.Domain.Events;
using QRCoder;

namespace BiteDanceAPI.Application.RedCodes.EventHandlers;

public class RedCodeRequestApprovedEventHandler(IEmailService emailService)
    : INotificationHandler<RedCodeRequestApprovedEvent>
{
    public async Task Handle(
        RedCodeRequestApprovedEvent notification,
        CancellationToken cancellationToken
    )
    {
        // Generate QR code
        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(
            $"r:{notification.ScanCode.RedCodeId}",
            QRCodeGenerator.ECCLevel.Q
        );
        Base64QRCode qrCode = new(qrCodeData);
        string qrCodeBase64 = qrCode.GetGraphic(
            20,
            darkColor: Color.DarkRed,
            lightColor: Color.White
        );

        var emailMessage = new EmailMsg
        {
            To = [notification.RedCodeRequest.WorkEmail],
            Cc = [notification.Approver.Email],
            Subject = "Red Code Request Approved",
            Body =
                $@"
                Your red code request has been approved. Please use the QR code below to scan in.
                <br>
                Note: {notification.RedCodeRequest.Note}
                <br><br>
                <img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' />
            "
        };

        await emailService.SendEmailAsync(emailMessage);
    }
}
