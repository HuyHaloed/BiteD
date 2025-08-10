namespace BiteDanceAPI.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailMsg msg);
}

public class EmailMsg
{
    public List<string> To { get; set; } = [];
    public List<string> Cc { get; set; } = [];
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public List<EmailAttachment> Attachments { get; set; } = new(); 
}


public class EmailAttachment
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public byte[] Content { get; set; } = default!;
}