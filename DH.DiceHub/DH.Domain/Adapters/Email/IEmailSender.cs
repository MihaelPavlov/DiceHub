using DH.Domain.Adapters.EmailSender;

namespace DH.Domain.Adapters.Email;

public interface IEmailSender
{
    bool SendEmail(EmailMessage message);
}
