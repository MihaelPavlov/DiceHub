using DH.Domain.Adapters.Email;
using DH.Domain.Entities;

namespace DH.Domain.Services;

public interface IEmailHelperService
{
    Task<EmailTemplate?> GetEmailTemplate(EmailType emailType);
    string LoadTemplate(string template, Dictionary<string, string> placeholders);
    Task CreateEmailHistory(EmailHistory history);
}
