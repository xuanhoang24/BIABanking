using BankingSystemAPI.Application.Services.Interfaces.Email;
using BankingSystemAPI.Domain.Entities.Email;
using Microsoft.Extensions.Options;
using Resend;

namespace BankingSystemAPI.Application.Services.Implementations.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly IResend _resend;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, IResend resend)
        {
            _settings = settings.Value;
            _logger = logger;
            _resend = resend;

            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                _logger.LogWarning("Resend API key is not configured. Emails will not be sent.");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                _logger.LogWarning("Resend not configured. Skipping email to {Email}", toEmail);
                return;
            }

            try
            {
                var message = new EmailMessage
                {
                    From = $"{_settings.FromName} <{_settings.FromEmail}>",
                    To = toEmail,
                    Subject = subject,
                    HtmlBody = htmlBody
                };

                var response = await _resend.EmailSendAsync(message);
                _logger.LogInformation("Email sent successfully to {Email}, Id: {Id}", toEmail, response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        public async Task SendVerificationEmailAsync(string toEmail, string firstName, string verificationToken, string baseUrl)
        {
            var verificationUrl = $"{baseUrl}/Auth/VerifyEmail?token={Uri.EscapeDataString(verificationToken)}";

            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "EmailTemplates", "VerificationEmail.html");
            var fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "EmailTemplates", "VerificationEmailFallback.html");
            
            string htmlBody;
            if (File.Exists(templatePath))
            {
                htmlBody = await File.ReadAllTextAsync(templatePath);
                _logger.LogInformation("Loaded email template from {Path}", templatePath);
            }
            else if (File.Exists(fallbackPath))
            {
                htmlBody = await File.ReadAllTextAsync(fallbackPath);
                _logger.LogWarning("Primary template not found, using fallback template from {Path}", fallbackPath);
            }
            else
            {
                _logger.LogError("No email templates found at {Path} or {FallbackPath}", templatePath, fallbackPath);
                throw new FileNotFoundException("Email templates not found");
            }

            htmlBody = htmlBody.Replace("{{FirstName}}", firstName);
            htmlBody = htmlBody.Replace("{{VerificationUrl}}", verificationUrl);

            await SendEmailAsync(toEmail, "Verify Your BIABank Account", htmlBody);
        }
    }
}
