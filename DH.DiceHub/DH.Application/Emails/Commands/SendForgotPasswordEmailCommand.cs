using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DH.Application.Emails.Commands;

public record SendForgotPasswordEmailCommand(string Email, string? Language) : IRequest;

internal class SendForgotPasswordEmailCommandHandler(
    ILogger<SendForgotPasswordEmailCommandHandler> logger,
    ITenantSettingsCacheService tenantSettingsCacheService,
    IUserManagementService userManagementService,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration,
    ILocalizationService localizationService) : IRequestHandler<SendForgotPasswordEmailCommand>
{
    readonly ILogger<SendForgotPasswordEmailCommandHandler> logger = logger;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly IUserManagementService userManagementService = userManagementService;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;
    readonly ILocalizationService localizationService = localizationService;

    public async Task Handle(SendForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManagementService.GetUserByEmail(request.Email);
        var emailType = EmailType.ForgotPasswordReset;
        var currentPreferredLanguage = request.Language ?? SupportLanguages.EN.ToString();

        if (user == null)
        {
            this.logger.LogWarning("User with Email {Email} was not found. {EmailType} was not send",
                request.Email,
                emailType);
            throw new ValidationErrorsException("Email", this.localizationService["ForgotPasswordUserWithEmailNotFound"]);
        }

        var emailTemplate = await this.emailHelperService.GetEmailTemplate(emailType, currentPreferredLanguage);
        if (emailTemplate == null)
        {
            this.logger.LogWarning("Email Template with Key {EmailType} was not found. {EmailType} was not send",
                emailType, emailType);
            return;
        }

        var settings = await tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        var token = await this.userManagementService.GeneratePasswordResetTokenAsync(request.Email);
        var encodedToken = WebUtility.UrlEncode(token);
        var frontendUrl = configuration.GetSection("Frontend_URL").Value;
        var callbackUrl = $"{frontendUrl}/reset-password?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}&language={currentPreferredLanguage}";

        var body = this.emailHelperService.LoadTemplate(emailTemplate.TemplateHtml, new Dictionary<string, string>
        {
            { ForgotPasswordResetKeys.CallbackUrl, callbackUrl },
            { ForgotPasswordResetKeys.ClubName, settings.ClubName }
        });

        var isEmailSendSuccessfully = this.emailSender.SendEmail(new EmailMessage
        {
            To = user.Email!,
            Subject = emailTemplate.Subject,
            Body = body
        });

        await this.emailHelperService.CreateEmailHistory(new EmailHistory
        {
            IsSuccessfully = isEmailSendSuccessfully,
            Body = body,
            SendedOn = DateTime.UtcNow,
            Subject = emailTemplate.Subject,
            TemplateName = emailTemplate.TemplateName,
            TemplateType = emailType.ToString(),
            To = user.Email,
            UserId = user.Id,
        });
    }
}
