using Identity_Server.Entities;
using MailKit.Net.Smtp;
using MimeKit;

namespace Identity_Server.Services;

public class SMTPEmailSender : IEmailSender
{
    private readonly IConfiguration configuration;

    public SMTPEmailSender(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(MailData mailData)
    {
        try
        {

            using(MimeMessage emailMessage = new MimeMessage())
            {
                var from = new MailboxAddress("sisakilcste18", "identity@twitter.com");
                var to = new MailboxAddress(mailData.Receiver, mailData.Receiver);

                emailMessage.From.Add(from);
                emailMessage.To.Add(to);

                emailMessage.Subject = mailData.Subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = mailData.Body;

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using(var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(configuration["MailtrapSettings:SMTP:Server"], 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(configuration["MailtrapSettings:SMTP:UserName"], configuration["MailtrapSettings:SMTP:Password"]);
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
