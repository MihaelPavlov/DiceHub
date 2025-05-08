using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DH.Adapter.Email;

public class SmtpEmailSender(IOptions<EmailSettings> mailSettings, ILogger<SmtpEmailSender> logger) : IEmailSender
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
                Body = message.Body,
                IsBodyHtml = true,
            };

            client.Send(mailMessage);

            return true;
        }
        catch (Exception ex)
        {
            // Exception Details

            return false;
        }
    }
}
