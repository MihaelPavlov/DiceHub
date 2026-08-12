using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;

namespace DH.Application.Common.Commands;

public record SendTenantSetupInvitationCommand(int TenantApplicationId) : IRequest<bool>;

internal class SendTenantSetupInvitationCommandHandler(
    ILogger<SendTenantSetupInvitationCommandHandler> logger,
    IRepository<TenantApplication> tenantApplicationRepository,
    IRepository<TenantSetupToken> tenantSetupTokenRepository,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration,
    ISystemUserContextAccessor systemUserContextAccessor) : IRequestHandler<SendTenantSetupInvitationCommand, bool>
{
    const int SetupTokenExpirationHours = 24;

    readonly ILogger<SendTenantSetupInvitationCommandHandler> logger = logger;
    readonly IRepository<TenantApplication> tenantApplicationRepository = tenantApplicationRepository;
    readonly IRepository<TenantSetupToken> tenantSetupTokenRepository = tenantSetupTokenRepository;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;
    readonly ISystemUserContextAccessor systemUserContextAccessor = systemUserContextAccessor;

    public async Task<bool> Handle(SendTenantSetupInvitationCommand request, CancellationToken cancellationToken)
    {
        var application = await this.tenantApplicationRepository.GetByAsync(
            x => x.Id == request.TenantApplicationId,
            cancellationToken)
            ?? throw new NotFoundException(nameof(TenantApplication), request.TenantApplicationId);

        if (application.Status != TenantApplicationStatus.Verified)
            throw new BadRequestException("Tenant setup invitation can only be sent for verified tenant applications.");

        var rawToken = CreateSecureToken();
        var setupToken = new TenantSetupToken
        {
            TenantApplicationId = application.Id,
            Email = NormalizeEmail(application.Email),
            TokenHash = HashToken(rawToken),
            CreatedDate = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(SetupTokenExpirationHours),
        };

        await this.tenantSetupTokenRepository.AddAsync(setupToken, cancellationToken);

        var frontendUrl = this.configuration.GetSection("Frontend_URL").Value?.TrimEnd('/') ?? string.Empty;
        var setupUrl = $"{frontendUrl}/tenant-setup?token={WebUtility.UrlEncode(rawToken)}";
        var emailType = EmailType.TenantSetupInvitation;
        var emailTemplate = await this.emailHelperService.GetEmailTemplate(emailType, SupportLanguages.EN.ToString());

        var bodyTemplate = emailTemplate?.TemplateHtml ?? GetFallbackTemplate();
        var subjectTemplate = emailTemplate?.Subject ?? "Your DiceHub venue setup is ready";

        var body = this.emailHelperService.LoadTemplate(bodyTemplate, new Dictionary<string, string>
        {
            { TenantSetupInvitation.ClubName, application.ContactName },
            { TenantSetupInvitation.SetupUrl, setupUrl },
            { TenantSetupInvitation.ExpiresInHours, SetupTokenExpirationHours.ToString() },
        });

        var subject = this.emailHelperService.LoadTemplate(subjectTemplate, new Dictionary<string, string>
        {
            { TenantSetupInvitation.ClubName, application.ContactName },
            { TenantSetupInvitation.ExpiresInHours, SetupTokenExpirationHours.ToString() },
        });

        var isEmailSendSuccessfully = this.emailSender.SendEmail(new EmailMessage
        {
            To = application.Email,
            Subject = subject,
            Body = body,
        });

        this.systemUserContextAccessor.Set(new TenantSetupInvitationSystemUserContext());
        await this.emailHelperService.CreateEmailHistory(new EmailHistory
        {
            IsSuccessfully = isEmailSendSuccessfully,
            Body = body,
            SendedOn = DateTime.UtcNow,
            Subject = subject,
            TemplateName = emailTemplate?.TemplateName ?? emailType.ToString(),
            TemplateType = emailType.ToString(),
            To = application.Email,
            UserId = "tenant-setup-invitation",
        });

        if (!isEmailSendSuccessfully)
        {
            this.logger.LogWarning(
                "Tenant setup invitation was not sent for tenant application {TenantApplicationId} to {Email}.",
                application.Id,
                application.Email);
        }

        return isEmailSendSuccessfully;
    }

    static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    static string CreateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    static string HashToken(string token)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    static string GetFallbackTemplate() =>
        """
        <!DOCTYPE html>
        <html lang="en">
          <body style="margin:0;padding:0;background-color:#20232a;font-family:Arial,sans-serif;color:white;">
            <table width="100%" cellpadding="0" cellspacing="0" role="presentation" style="background-color:#20232a;padding:20px 0;">
              <tr>
                <td align="center">
                  <table width="520" cellpadding="0" cellspacing="0" role="presentation" style="background-color:#20232a;padding:20px;border-radius:8px;">
                    <tr>
                      <td align="center" style="font-size:22px;font-weight:bold;padding-bottom:20px;">
                        DiceHub venue setup
                      </td>
                    </tr>
                    <tr>
                      <td style="font-size:16px;line-height:1.6;">
                        <p>Your venue application for {{ClubName}} was approved.</p>
                        <p>Use the secure link below to start setting up your tenant:</p>
                        <p><a href="{{SetupUrl}}" style="color:#75a0ff;">Start tenant setup</a></p>
                        <p>This link expires in {{ExpiresInHours}} hours.</p>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>
          </body>
        </html>
        """;

    private sealed class TenantSetupInvitationSystemUserContext : IUserContext
    {
        public string? TenantId => null;
        public string? UserId => "tenant-setup-invitation";
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => false;
        public bool IsSystem => true;
    }
}
