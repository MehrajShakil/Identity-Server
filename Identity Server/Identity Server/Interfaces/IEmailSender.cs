using Identity_Server.Entities;

namespace Identity_Server.Interfaces;

public interface IEmailSender
{
    Task<bool> SendEmailAsync(MailData mailData);
}
