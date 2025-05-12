using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Entities;
using DH.Domain.Services;
using DH.Domain.Services.TenantSettingsService;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DH.Application.Emails.Commands;

public record SendForgotPasswordEmailCommand(string Email) : IRequest;

internal class SendForgotPasswordEmailCommandHandler(
    ILogger<SendForgotPasswordEmailCommandHandler> logger,
    ITenantSettingsCacheService tenantSettingsCacheService,
    IUserService userService,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration) : IRequestHandler<SendForgotPasswordEmailCommand>
{
    readonly ILogger<SendForgotPasswordEmailCommandHandler> logger = logger;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly IUserService userService = userService;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;

    public async Task Handle(SendForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userService.GetUserByEmail(request.Email);

        if (user == null)
        {
            this.logger.LogWarning("User with Email {Email} was not found. {EmailType} was not send",
                request.Email,
                EmailType.ForgotPasswordReset.ToString());
            throw new ValidationErrorsException("Email", "User with this email adrress doesn't exists!");
        }

        var emailTemplate = await this.emailHelperService.GetEmailTemplate(EmailType.ForgotPasswordReset);
        if (emailTemplate == null)
        {
            var emailType = EmailType.ForgotPasswordReset.ToString();
            this.logger.LogWarning("Email Template with Key {EmailType} was not found. {EmailType} was not send",
                emailType, emailType);
            return;
        }

        var settings = await tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        var token = await this.userService.GeneratePasswordResetTokenAsync(request.Email);
        var encodedToken = WebUtility.UrlEncode(token);
        var frontendUrl = configuration.GetSection("Frontend_URL").Value;
        var callbackUrl = $"{frontendUrl}/reset-password?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";

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
            To = user.Email,
            UserId = user.Id,
        });
    }
}
