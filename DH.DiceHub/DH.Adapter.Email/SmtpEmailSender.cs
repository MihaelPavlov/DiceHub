using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace DH.Adapter.Email;

public class SmtpEmailSender(
    IOptions<EmailSettings> mailSettings,
    ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailSettings mailSettings = mailSettings.Value;
    private readonly ILogger<SmtpEmailSender> logger = logger;

    public bool SendEmail(EmailMessage message)
    {
        try
        {
            var client = new SmtpClient(mailSettings.Host, mailSettings.Port)
            {
                Credentials = new NetworkCredential(mailSettings.UserName, mailSettings.Password),
                EnableSsl = mailSettings.UseSSL
            };

            var mailMessage = new MailMessage(mailSettings.EmailId!, message.To)
            {
                From = new MailAddress(mailSettings.EmailId!, mailSettings.Name),
                Subject = message.Subject,
                // A pure HTML body with no plain-text fallback is a well-known spam
                // signal (no multipart/alternative), so the plain-text part below is
                // set as the primary Body and the HTML is attached as an
                // AlternateView - the standard System.Net.Mail pattern for producing
                // a proper multipart/alternative message.
                Body = HtmlToPlainText(message.Body),
                IsBodyHtml = false,
            };
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html"));

            client.Send(mailMessage);

            return true;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to send email to {Email}, ex {ex}, innerEx: {innerEx}", message.To, ex.Message, ex.InnerException);

            return false;
        }
    }

    private static string HtmlToPlainText(string html)
    {
        var withoutTags = Regex.Replace(html, "<(script|style)[^>]*>.*?</\\1>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        withoutTags = Regex.Replace(withoutTags, "<br\\s*/?>|</p>|</div>", "\n", RegexOptions.IgnoreCase);
        withoutTags = Regex.Replace(withoutTags, "<[^>]+>", string.Empty);
        var decoded = WebUtility.HtmlDecode(withoutTags);

        return Regex.Replace(decoded, "\n{3,}", "\n\n").Trim();
    }
}
