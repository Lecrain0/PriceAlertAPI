using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PriceAlertAPI.Services.Interfaces;

namespace PriceAlertAPI.Services;

public class EmailNotificationService : INotificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(IConfiguration config, ILogger<EmailNotificationService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendPriceAlertEmailAsync(
        string toEmail, string productName, decimal targetPrice, decimal currentPrice)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:Username"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = $"🎉 Fiyat Düştü: {productName}";

            email.Body = new TextPart("html")
            {
                Text = $"""
                    <h2>Fiyat Alarmı Tetiklendi!</h2>
                    <p><strong>{productName}</strong> için belirlediğin hedef fiyata ulaşıldı.</p>
                    <table>
                        <tr><td>Hedef Fiyat:</td><td><strong>{targetPrice:C2}</strong></td></tr>
                        <tr><td>Güncel Fiyat:</td><td><strong style="color:green">{currentPrice:C2}</strong></td></tr>
                    </table>
                    <br/>
                    <p>Hemen satın almak için ürün sayfasını ziyaret edebilirsin.</p>
                    <hr/>
                    <small>Bu e-posta PriceAlertAPI tarafından gönderilmiştir.</small>
                """
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["Email:SmtpHost"],
                int.Parse(_config["Email:SmtpPort"]!),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _config["Email:Username"],
                _config["Email:Password"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email gönderildi: {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email gönderilemedi: {Email}", toEmail);
        }
    }
}
