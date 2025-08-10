using BiteDanceAPI.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace BiteDanceAPI.Infrastructure.Email;

public class ConsoleEmailService(ILogger<ConsoleEmailService> logger) : IEmailService
{
    public Task SendEmailAsync(EmailMsg msg)
    {
        logger.LogInformation("Email sent: {@message}", msg);

        return Task.CompletedTask;
    }
}
