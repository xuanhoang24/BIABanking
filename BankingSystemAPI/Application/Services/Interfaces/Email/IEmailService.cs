namespace BankingSystemAPI.Application.Services.Interfaces.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendVerificationEmailAsync(string toEmail, string firstName, string verificationToken, string baseUrl);
    }
}
