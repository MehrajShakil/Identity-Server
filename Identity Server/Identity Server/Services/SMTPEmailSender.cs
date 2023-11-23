using Identity_Server.Entities;
using Identity_Server.Interfaces;
using Identity_Server.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Identity_Server.Services;

public class SMTPEmailSender : IEmailSender
{
    private readonly IConfiguration configuration;
    private readonly IOptionsFactory<SmtpEmailSenderOptions> emailSender;

    public SMTPEmailSender(IConfiguration configuration, IOptionsFactory<SmtpEmailSenderOptions> smtpEmailSenderOptions)
    {
        this.configuration = configuration;
        smtpEmailSenderOptions.Create(SmtpEmailSenderOptions.SendGrid);
    }

    public async Task<bool> SendEmailAsync(MailData mailData)
    {
        try
        {

            using(MimeMessage emailMessage = new MimeMessage())
            {
                var from = new MailboxAddress("twitter","twitter@gmail.com");
                var to = new MailboxAddress(mailData.ReceiverEmail, mailData.ReceiverEmail);

                emailMessage.From.Add(from);
                emailMessage.To.Add(to);

                emailMessage.Subject = mailData.Subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = mailData.Body;

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using(var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(configuration["SendGrid:SMTP:Server"], 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(configuration["SendGrid:SMTP:UserName"], configuration["SendGrid:SMTP:Password"]);
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
