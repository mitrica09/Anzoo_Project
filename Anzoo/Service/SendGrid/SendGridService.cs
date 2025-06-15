using Anzoo.Repository.SendGrid;
using Anzoo.Service.SendGrid;

public class SendGridService : ISendGridService
{
    private readonly IConfiguration _config;
    private readonly ISendGridRepository _repository;

    public SendGridService(IConfiguration config, ISendGridRepository repository)
    {
        _config = config;
        _repository = repository;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var subject = "Reset your Anzoo password";
        var htmlContent = $@"
            <div style='font-family:Arial,sans-serif'>
                <h2>Resetare parolă Anzoo</h2>
                <p>Dă clic pe butonul de mai jos pentru a-ți reseta parola:</p>
                <p>
                    <a href='{resetLink}' style='background:#28a745;color:#fff;
                       padding:10px 20px;text-decoration:none;border-radius:4px;'>
                        Schimbă parola
                    </a>
                </p>
                <p>Dacă nu ai cerut acest lucru, ignoră acest mesaj.</p>
                <small>Echipa Anzoo</small>
            </div>";

        await SendEmailAsync(toEmail, subject, htmlContent);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var fromEmail = _config["SendGrid:FromEmail"];
        var fromName = _config["SendGrid:FromName"];

        await _repository.SendEmailAsync(toEmail, fromEmail, fromName, subject, htmlBody);
    }
}
