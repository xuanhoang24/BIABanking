using System.Net;
using System.Net.Mail;
using BankingSystemAPI.Application.Services.Interfaces.Email;
using BankingSystemAPI.Domain.Entities.Email;
using Microsoft.Extensions.Options;

namespace BankingSystemAPI.Application.Services.Implementations.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            // Validate settings
            if (string.IsNullOrEmpty(_settings.SmtpHost))
            {
                _logger.LogWarning("Email service is not configured. Emails will not be sent.");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            // Skip if not configured
            if (string.IsNullOrEmpty(_settings.SmtpHost) || string.IsNullOrEmpty(_settings.SmtpUsername))
            {
                _logger.LogWarning("Email service not configured. Skipping email to {Email}", toEmail);
                return;
            }

            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(_settings.FromEmail, _settings.FromName);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = htmlBody;
                message.IsBodyHtml = true;

                using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort);
                client.EnableSsl = _settings.EnableSsl;
                client.Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
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

            // Load email template
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

            // Replace placeholders
            htmlBody = htmlBody.Replace("{{FirstName}}", firstName);
            htmlBody = htmlBody.Replace("{{VerificationUrl}}", verificationUrl);

            await SendEmailAsync(toEmail, "Verify Your BIABank Account", htmlBody);
        }
    }
}
