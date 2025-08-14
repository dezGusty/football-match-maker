using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FootballAPI.Service.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }


    public async Task SendSetPasswordEmailAsync(string email, string username, string setPasswordUrl)
    {
        try
        {
            var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
            {
                Port = int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                Credentials = new NetworkCredential(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                ),
                EnableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:FromAddress"] ?? "noreply@yourapp.com",
                                     _configuration["Email:FromName"] ?? "Football App"),
                Subject = "Set Your Password - Football App",
                Body = GenerateSetPasswordEmailBody(username, setPasswordUrl),
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Password setup email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password setup email to {Email}", email);
            throw new InvalidOperationException("Failed to send email", ex);
        }
    }
        
        private string GenerateSetPasswordEmailBody(string username, string setPasswordUrl)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #343a40; text-align: center;'>Welcome to Football App!</h2>
                        
                        <p>Hello <strong>{username}</strong>,</p>
                        
                        <p>An administrator has created an account for you on Football App. To complete your account setup, you need to set your password.</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{setPasswordUrl}' 
                               style='background-color: #007bff; color: white; padding: 12px 30px; 
                                      text-decoration: none; border-radius: 5px; display: inline-block;
                                      font-weight: bold;'>
                                Set Your Password
                            </a>
                        </div>
                        
                        <p><strong>Important:</strong></p>
                        <ul>
                            <li>This link is valid for 24 hours</li>
                            <li>You can only use this link once</li>
                            <li>If the link expires, contact an administrator for a new one</li>
                        </ul>
                        
                        <p>If the button above doesn't work, copy and paste this link into your browser:</p>
                        <p style='background-color: #e9ecef; padding: 10px; border-radius: 5px; word-break: break-all;'>
                            {setPasswordUrl}
                        </p>
                        
                        <hr style='margin: 30px 0;'>
                        
                        <p style='font-size: 12px; color: #6c757d; text-align: center;'>
                            If you didn't expect this email, please ignore it or contact support.
                        </p>
                    </div>
                </body>
                </html>";
        }


}


