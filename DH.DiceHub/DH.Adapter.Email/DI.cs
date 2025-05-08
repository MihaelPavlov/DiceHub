using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.Email;

public static class DI
{
    public static void AddEmailAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        var emailSettings =configuration.GetSection("MailSettings").Get<EmailSettings>()
            ?? throw new Exception("Failed to load MailSettings configuration. Ensure 'MailSettings' section exists in appsettings.json.");

        services.Configure<EmailSettings>(configuration.GetSection("MailSettings"));
        services.AddScoped<IEmailSender, SmtpEmailSender>();
    }
}
