using DH.Domain.Adapters.Email;
using DH.Domain.Entities;
using DH.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Services;

internal class EmailHelperService(IDbContextFactory<TenantDbContext> dbContextFactory) : IEmailHelperService
{
    readonly IDbContextFactory<TenantDbContext> _contextFactory = dbContextFactory;

    public async Task CreateEmailHistory(EmailHistory history)
    {
        using (var context = await this._contextFactory.CreateDbContextAsync())
        {
            context.EmailHistory.Add(history);
            await context.SaveChangesAsync();
        }
    }

    public async Task<EmailTemplate?> GetEmailTemplate(EmailType emailType)
    {
        using (var context = await this._contextFactory.CreateDbContextAsync())
        {
            return await context.EmailTemplates
                .FirstOrDefaultAsync(x => x.TemplateName == emailType.ToString());
        }
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
