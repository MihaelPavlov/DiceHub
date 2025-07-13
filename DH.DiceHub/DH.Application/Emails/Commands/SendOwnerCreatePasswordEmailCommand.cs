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

public record SendOwnerCreatePasswordEmailCommand(string Email) : IRequest;

internal class SendOwnerCreatePasswordEmailCommandHandler(
    ILogger<SendOwnerCreatePasswordEmailCommandHandler> logger,
    ITenantSettingsCacheService tenantSettingsCacheService,
    IUserService userService,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration) : IRequestHandler<SendOwnerCreatePasswordEmailCommand>
{
    readonly ILogger<SendOwnerCreatePasswordEmailCommandHandler> logger = logger;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly IUserService userService = userService;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;

    public async Task Handle(SendOwnerCreatePasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userService.GetUserByEmail(request.Email);
        var emailType = EmailType.OwnerPasswordCreation;
        if (user == null)
        {
            this.logger.LogWarning("User with Email {Email} was not found. {EmailType} was not send",
                request.Email,
                emailType);
            throw new ValidationErrorsException("Email", "User with this email adrress doesn't exists!");
        }

        var emailTemplate = await this.emailHelperService.GetEmailTemplate(emailType);
        if (emailTemplate == null)
        {
            this.logger.LogWarning("Email Template with Key {EmailType} was not found. {EmailType} was not send",
                emailType, emailType);
            return;
        }

        var settings = await tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        var token = await this.userService.GeneratePasswordResetTokenAsync(request.Email);
        var encodedToken = WebUtility.UrlEncode(token);
        var frontendUrl = configuration.GetSection("Frontend_URL").Value;
        var callbackUrl = $"{frontendUrl}/create-owner-password?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";

        var body = this.emailHelperService.LoadTemplate(emailTemplate.TemplateHtml, new Dictionary<string, string>
        {
            { OwnerPasswordCreation.CreatePasswordUrl, callbackUrl },
            { OwnerPasswordCreation.ClubName, settings.ClubName },
        });

        var subject = this.emailHelperService.LoadTemplate(emailTemplate.Subject, new Dictionary<string, string>
        {
            { OwnerPasswordCreation.ClubName, settings.ClubName },
        });

        var isEmailSendSuccessfully = this.emailSender.SendEmail(new EmailMessage
        {
            To = user.Email!,
            Subject = subject,
            Body = body
        });

        await this.emailHelperService.CreateEmailHistory(new EmailHistory
        {
            IsSuccessfully = isEmailSendSuccessfully,
            Body = body,
            SendedOn = DateTime.UtcNow,
            Subject = subject,
            TemplateName = emailTemplate.TemplateName,
            TemplateType = emailType.ToString(),
            To = user.Email,
            UserId = user.Id,
        });
    }
}