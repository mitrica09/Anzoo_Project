using System.Text.Json;
using System.Text;
using Anzoo.Repository.SendGrid;

public class SendGridRepository : ISendGridRepository
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public SendGridRepository(IConfiguration config)
    {
        _config = config;
        _httpClient = new HttpClient();
    }

    public async Task SendEmailAsync(string toEmail, string fromEmail, string fromName, string subject, string htmlContent)
    {
        var payload = new
        {
            personalizations = new[] {
                new {
                    to = new[] { new { email = toEmail } },
                    subject
                }
            },
            from = new { email = fromEmail, name = fromName },
            content = new[] {
                new { type = "text/html", value = htmlContent }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
        request.Headers.Add("Authorization", $"Bearer {_config["SendGrid:ApiKey"]}");
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"SendGrid error {response.StatusCode}: {error}");
        }
    }
}
