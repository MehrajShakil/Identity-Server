using Identity_Server.Settings;

namespace Identity_Server.Extensions;

public static class AddConfigurationSettingsExtension
{
    public static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpEmailSenderOptions>(SmtpEmailSenderOptions.Local,
            configuration.GetSection(SmtpEmailSenderOptions.GetLocalServerPath()));

        services.Configure<SmtpEmailSenderOptions>(SmtpEmailSenderOptions.SendGrid,
            configuration.GetSection(SmtpEmailSenderOptions.GetSendGridServerPath()));

        services.Configure<SmtpEmailSenderOptions>(SmtpEmailSenderOptions.Mailtrap,
            configuration.GetSection(SmtpEmailSenderOptions.GetMailtrapServerPath()));
    }
}

