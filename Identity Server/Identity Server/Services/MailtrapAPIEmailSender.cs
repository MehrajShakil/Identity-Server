using Identity_Server.Entities;
using Newtonsoft.Json;

namespace Identity_Server.Interfaces;

public class MailtrapAPIEmailSender : IEmailSender
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;

    public MailtrapAPIEmailSender(IConfiguration configuration, 
                                  IHttpClientFactory httpClientFactory)
    {
        this.configuration = configuration;
        this.httpClient = httpClientFactory.CreateClient("MailTrapApiClient");
    }

    public async Task<bool> SendEmailAsync(MailData mailData)
    {

        var apiMail = new
        {
            From = new {Email = mailData.Sender, Name = mailData.Sender},
            To = new { Email = mailData.Receiver, Name = mailData.Receiver },
            Subject = mailData.Subject,
            Text = mailData.Body
        };

        var httpResponse = await httpClient.PostAsJsonAsync("send", apiMail);
        var responseString = await httpResponse.Content.ReadAsStringAsync();

        var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);

        if(response.TryGetValue("Success", out var res))
        {
            if ((bool)res) return true;
            return false;
        }

        return false;

    }
}
