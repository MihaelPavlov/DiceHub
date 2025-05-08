using DH.Domain.Adapters.Email;
using DH.Domain.Entities;
using DH.Domain.Services;

namespace DH.Adapter.Data.Services;

internal class EmailHelperService : IEmailHelperService
{
    public Task CreateEmailHistory(EmailHistory history)
    {
        throw new NotImplementedException();
    }

    public Task<EmailTemplate> GetEmailTemplate(EmailType emailType)
    {
        throw new NotImplementedException();
    }

    public string LoadTemplate(string template, Dictionary<string, string> placeholders)
    {
        foreach (var kvp in placeholders)
        {
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value); // replaces {{Key}} with Value
        }

        return template;
    }
}
