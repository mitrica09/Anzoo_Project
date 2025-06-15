namespace Anzoo.Service.SendGrid
{
    public interface ISendGridService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }


}
