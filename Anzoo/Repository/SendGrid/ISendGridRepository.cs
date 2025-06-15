namespace Anzoo.Repository.SendGrid
{
    public interface ISendGridRepository
    {
        Task SendEmailAsync(string toEmail, string fromEmail, string fromName, string subject, string htmlContent);
    }
}
