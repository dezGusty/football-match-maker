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

    private async Task SendEmailAsync(MailMessage mailMessage)
    {
        var smtpClient = new SmtpClient(_configuration["SmtpSettings:Host"])
        {
            Port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587"),
            Credentials = new NetworkCredential(
                _configuration["SmtpSettings:Username"],
                _configuration["SmtpSettings:Password"]
            ),
            EnableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"] ?? "true")
        };

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendSetPasswordEmailAsync(string email, string username, string setPasswordUrl)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:FromAddress"] ?? "noreply@yourapp.com",
                                     _configuration["Email:FromName"] ?? "Football App"),
                Subject = "Set Your Password - Football App",
                Body = GenerateSetPasswordEmailBody(username, setPasswordUrl),
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);
            await SendEmailAsync(mailMessage);

            _logger.LogInformation("Password setup email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password setup email to {Email}", email);
            throw new InvalidOperationException("Failed to send email", ex);
        }
    }
    public async Task SendPasswordResetEmailAsync(string email, string username, string resetPasswordUrl)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:FromAddress"] ?? "noreply@yourapp.com",
                                     _configuration["Email:FromName"] ?? "Football App"),
                Subject = "Reset Your Password - Football App",
                Body = GeneratePasswordResetEmailBody(username, resetPasswordUrl),
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);
            await SendEmailAsync(mailMessage);

            _logger.LogInformation("Password reset email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
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

    private string GeneratePasswordResetEmailBody(string username, string resetPasswordUrl)
    {
        return $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #343a40; text-align: center;'>Reset Your Password</h2>
                        
                        <p>Hello <strong>{username}</strong>,</p>
                        
                        <p>We received a request to reset your password for your Football App account. If you didn't make this request, you can safely ignore this email.</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{resetPasswordUrl}' 
                               style='background-color: #dc3545; color: white; padding: 12px 30px; 
                                      text-decoration: none; border-radius: 5px; display: inline-block;
                                      font-weight: bold;'>
                                Reset Your Password
                            </a>
                        </div>
                        
                        <p><strong>Security Information:</strong></p>
                        <ul>
                            <li>This link is valid for 24 hours</li>
                            <li>You can only use this link once</li>
                            <li>After using this link, you'll be able to set a new password</li>
                        </ul>
                        
                        <p>If the button above doesn't work, copy and paste this link into your browser:</p>
                        <p style='background-color: #e9ecef; padding: 10px; border-radius: 5px; word-break: break-all;'>
                            {resetPasswordUrl}
                        </p>
                        
                        <div style='background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <p style='margin: 0; color: #856404;'>
                                <strong>⚠️ Security Tip:</strong> If you didn't request this password reset, 
                                please contact our support team immediately as someone may be trying to access your account.
                            </p>
                        </div>
                        
                        <hr style='margin: 30px 0;'>
                        
                        <p style='font-size: 12px; color: #6c757d; text-align: center;'>
                            This email was sent because a password reset was requested for your account.<br>
                            If you didn't make this request, please ignore this email or contact support.
                        </p>
                    </div>
                </body>
                </html>";
    }

}


