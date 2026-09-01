using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DH.Application.Common.Commands;

public record SendTenantOwnerCredentialsEmailCommand(
    string TenantId,
    string ClubName,
    string OwnerEmail,
    string TemporaryPassword) : IRequest<bool>;

internal class SendTenantOwnerCredentialsEmailCommandHandler(
    ILogger<SendTenantOwnerCredentialsEmailCommandHandler> logger,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration,
    ISystemUserContextAccessor systemUserContextAccessor) : IRequestHandler<SendTenantOwnerCredentialsEmailCommand, bool>
{
    readonly ILogger<SendTenantOwnerCredentialsEmailCommandHandler> logger = logger;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;
    readonly ISystemUserContextAccessor systemUserContextAccessor = systemUserContextAccessor;

    public async Task<bool> Handle(SendTenantOwnerCredentialsEmailCommand request, CancellationToken cancellationToken)
    {
        var frontendUrl = this.configuration.GetSection("Frontend_URL").Value?.TrimEnd('/') ?? string.Empty;
        var loginUrl = $"{frontendUrl}/{request.TenantId}/login";
        var emailType = EmailType.TenantOwnerCredentials;
        var emailTemplate = await this.emailHelperService.GetEmailTemplate(emailType, SupportLanguages.EN.ToString());

        var bodyTemplate = emailTemplate?.TemplateHtml ?? GetFallbackTemplate();
        var subjectTemplate = emailTemplate?.Subject ?? "Your DiceHub owner account is ready";

        var replacements = new Dictionary<string, string>
        {
            { TenantOwnerCredentials.ClubName, WebUtility.HtmlEncode(request.ClubName) },
            { TenantOwnerCredentials.LoginUrl, WebUtility.HtmlEncode(loginUrl) },
            { TenantOwnerCredentials.Email, WebUtility.HtmlEncode(request.OwnerEmail) },
            { TenantOwnerCredentials.Password, WebUtility.HtmlEncode(request.TemporaryPassword) },
        };

        var body = this.emailHelperService.LoadTemplate(bodyTemplate, replacements);
        var subject = this.emailHelperService.LoadTemplate(subjectTemplate, replacements);

        var isEmailSendSuccessfully = this.emailSender.SendEmail(new EmailMessage
        {
            To = request.OwnerEmail,
            Subject = subject,
            Body = body,
        });

        this.systemUserContextAccessor.Set(new TenantOwnerCredentialsSystemUserContext(request.TenantId));
        await this.emailHelperService.CreateEmailHistory(new EmailHistory
        {
            IsSuccessfully = isEmailSendSuccessfully,
            Body = body,
            SendedOn = DateTime.UtcNow,
            Subject = subject,
            TemplateName = emailTemplate?.TemplateName ?? emailType.ToString(),
            TemplateType = emailType.ToString(),
            To = request.OwnerEmail,
            UserId = "tenant-owner-credentials",
        });

        if (!isEmailSendSuccessfully)
        {
            this.logger.LogWarning(
                "Tenant owner credentials email was not sent for tenant {TenantId} to {Email}.",
                request.TenantId,
                request.OwnerEmail);
        }

        return isEmailSendSuccessfully;
    }

    static string GetFallbackTemplate() =>
        """
        <!DOCTYPE html>
        <html lang="en">
          <body style="margin:0;padding:0;background-color:#20232a;font-family:Arial,sans-serif;color:white;">
            <table width="100%" cellpadding="0" cellspacing="0" role="presentation" style="background-color:#20232a;padding:20px 0;">
              <tr>
                <td align="center">
                  <table width="560" cellpadding="0" cellspacing="0" role="presentation" style="background-color:#20232a;padding:20px;border-radius:8px;">
                    <tr>
                      <td align="center" style="font-size:22px;font-weight:bold;padding-bottom:20px;">
                        Your DiceHub club is ready
                      </td>
                    </tr>
                    <tr>
                      <td style="font-size:16px;line-height:1.6;">
                        <p>Your owner account for <strong>{{ClubName}}</strong> has been created.</p>
                        <p>Sign in here: <a href="{{LoginUrl}}" style="color:#75a0ff;">{{LoginUrl}}</a></p>
                        <p><strong>Email:</strong> {{Email}}</p>
                        <p><strong>Temporary password:</strong> {{Password}}</p>
                        <p>Please change this password after signing in.</p>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>
          </body>
        </html>
        """;

    private sealed class TenantOwnerCredentialsSystemUserContext(string tenantId) : IUserContext
    {
        public string? TenantId => tenantId;
        public string? UserId => "tenant-owner-credentials";
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => false;
        public bool IsSystem => true;
    }
}
