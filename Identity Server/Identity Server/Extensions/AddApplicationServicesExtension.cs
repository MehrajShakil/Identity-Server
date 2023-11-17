using Identity_Server.Interfaces;
using Identity_Server.Services;

namespace Identity_Server.Extensions;

public static class AddApplicationServicesExtension
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("MailTrapApiClient", (service, client) =>
        {
            client.BaseAddress = new Uri(configuration["MailtrapSettings:ApiBaseUrl"]);
            client.DefaultRequestHeaders.Add("Api-Token", configuration["MailtrapSettings:ApiToken"]);
        });

        services.AddSingleton<IEmailSender, MailtrapSMTPEmailSender>();
        services.AddScoped<IAccountService, AccountService>();

    }
}