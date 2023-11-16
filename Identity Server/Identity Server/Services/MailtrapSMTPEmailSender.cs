using Identity_Server.Entities;
using Identity_Server.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace Identity_Server.Services;

public class MailtrapSMTPEmailSender : IEmailSender
{
    private readonly IConfiguration configuration;

    public MailtrapSMTPEmailSender(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(MailData mailData)
    {
        try
        {

            using(MimeMessage emailMessage = new MimeMessage())
            {
                var from = new MailboxAddress("twitter", configuration["LocalMailSettings:SenderEmail"]);
                var to = new MailboxAddress(mailData.ReceiverEmail, mailData.ReceiverEmail);

                emailMessage.From.Add(from);
                emailMessage.To.Add(to);

                emailMessage.Subject = mailData.Subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = mailData.Body;

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using(var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(configuration["LocalMailSettings:Server"], 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(configuration["LocalMailSettings:SenderEmail"], configuration["LocalMailSettings:Password"]);
                    await smtpClient.SendAsync(emailMessage);
                    await smtpClient.DisconnectAsync(true);
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
