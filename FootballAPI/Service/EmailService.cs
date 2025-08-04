using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _fromEmail = "foottballmatchmaker@gmail.com";
    private readonly string _fromPassword = "gjct lndg qgff sxrl";

    public async Task<bool> SendForgottenPasswordEmailAsync(string toEmail, string username, string newPassword)
{
    var subject = "Resetare Parolă";
    var body = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2>Salut, {username}!</h2>
    <p>Am primit o cerere de resetare a parolei pentru contul tău.</p>
    <p style='font-size:16px;'>Aceasta este noua ta parolă: 
        <strong style='color:blue;'>{newPassword}</strong>
    </p>
    <p>Cu drag,<br/>Echipa noastră</p>
</body>
</html>";

    return await SendEmailAsync(toEmail, subject, body);
}

private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
{
    try
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(_fromEmail, _fromPassword),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail),
            Subject = subject,
            SubjectEncoding = System.Text.Encoding.UTF8,
            Body = body,
            BodyEncoding = System.Text.Encoding.UTF8,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
        Console.WriteLine("[INFO] Email trimis cu succes.");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Eroare la trimiterea emailului: {ex.Message}");
        return false;
    }
}

    public string GenerateRandomPassword(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}