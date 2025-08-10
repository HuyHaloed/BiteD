using Azure;
using Azure.Communication.Email;
using BiteDanceAPI.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BiteDanceAPI.Infrastructure.Email
{
    public class AzureEmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly ILogger<AzureEmailService> _logger;
        private readonly string _sender;

        public AzureEmailService(IConfiguration configuration, ILogger<AzureEmailService> logger)
        {
            var connectionString = configuration["AzureCommunicationService:ConnectionString"];
            _emailClient = new EmailClient(connectionString);
            _logger = logger;
            _sender = configuration["AzureCommunicationService:Sender"] ?? "";
        }

        public async Task SendEmailAsync(EmailMsg msg)
        {
            // Ref: https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/send-email-advanced/send-email-to-multiple-recipients?tabs=connection-string&pivots=programming-language-csharp
            // Create the email content
            var emailContent = new EmailContent(msg.Subject)
            {
                PlainText = msg.Body,
                Html = msg.Body
            };

            // Create the To list
            var toRecipients = msg.To.Select(email => new EmailAddress(email)).ToList();

            // Create the CC list
            var ccRecipients = msg.Cc.Select(email => new EmailAddress(email)).ToList();

            EmailRecipients emailRecipients = new(toRecipients, ccRecipients, []);

            // Create the EmailMessage
            var emailMessage = new EmailMessage(
                senderAddress: _sender,
                emailRecipients,
                emailContent
            );

            try
            {
                EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                    WaitUntil.Completed,
                    emailMessage
                );
                _logger.LogInformation(
                    "Email Sent. Status = {status}",
                    emailSendOperation.Value.Status
                );

                // Get the OperationId so that it can be used for tracking the message for troubleshooting
                string operationId = emailSendOperation.Id;
                _logger.LogInformation("Email operation id = {operationId}", operationId);
            }
            catch (RequestFailedException ex)
            {
                // OperationID is contained in the exception message and can be used for troubleshooting purposes
                _logger.LogError(
                    "Email send operation failed with error code: {errorCode}, message: {message}",
                    ex.ErrorCode,
                    ex.Message
                );
            }
        }
    }
}
