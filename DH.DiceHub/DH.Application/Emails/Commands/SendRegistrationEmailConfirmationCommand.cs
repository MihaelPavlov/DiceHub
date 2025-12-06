using DH.Domain.Adapters.Authentication.Models;
using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DH.Application.Emails.Commands;

public record SendRegistrationEmailConfirmationCommand(string? ByUserId, string? ByEmail, string? Language) : IRequest<bool>;

internal class SendRegistrationEmailConfirmationCommandHandler(
    ILogger<SendRegistrationEmailConfirmationCommandHandler> logger,
    ITenantSettingsCacheService tenantSettingsCacheService,
    IUserService userService,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration) : IRequestHandler<SendRegistrationEmailConfirmationCommand, bool>
{
    readonly ILogger<SendRegistrationEmailConfirmationCommandHandler> logger = logger;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly IUserService userService = userService;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;

    public async Task<bool> Handle(SendRegistrationEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var emailType = EmailType.RegistrationEmailConfirmation;
        var currentPreferredLanguage = request.Language ?? SupportLanguages.EN.ToString();

        if (string.IsNullOrWhiteSpace(request.ByUserId) && string.IsNullOrWhiteSpace(request.ByEmail))
        {
            logger.LogWarning("Neither User ID nor Email was provided. {EmailType} was not sent.",
                emailType);
            return false;
        }

        UserModel? user = null;

        if (!string.IsNullOrWhiteSpace(request.ByUserId))
            user = await userService.GetUserById(request.ByUserId, cancellationToken);

        if (user == null && !string.IsNullOrWhiteSpace(request.ByEmail))
            user = await userService.GetUserByEmail(request.ByEmail);

        if (user == null)
        {
            var missingInfo = !string.IsNullOrWhiteSpace(request.ByUserId)
                ? $"ID: {request.ByUserId}"
                : $"Email: {request.ByEmail}";

            logger.LogWarning("User with {UserIdentifier} was not found. {EmailType} was not sent.",
                missingInfo,
                emailType);
            return false;
        }

        var emailTemplate = await this.emailHelperService.GetEmailTemplate(emailType, currentPreferredLanguage);
        if (emailTemplate == null)
        {
            this.logger.LogWarning("Email Template with Key {EmailType} was not found. {EmailType} was not send",
                emailType, emailType);
            return false;
        }

        var settings = await tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);
        var token = await this.userService.GenerateEmailConfirmationTokenAsync(user.Id);
        var encodedToken = WebUtility.UrlEncode(token);
        var frontendUrl = configuration.GetSection("Frontend_URL").Value;
        var callbackUrl = $"{frontendUrl}/confirm-email?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}&language={currentPreferredLanguage}";

        var body = this.emailHelperService.LoadTemplate(emailTemplate.TemplateHtml, new Dictionary<string, string>
        {
            { RegistrationEmailTemplateKeys.CallbackUrl, callbackUrl },
            { RegistrationEmailTemplateKeys.ClubName, settings.ClubName }
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

        if (!isEmailSendSuccessfully)
            return false;

        return true;
    }
}
