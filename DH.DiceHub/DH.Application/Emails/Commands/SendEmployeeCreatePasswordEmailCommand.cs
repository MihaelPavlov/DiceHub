using DH.Domain.Adapters.Authentication;
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

public record SendEmployeeCreatePasswordEmailCommand(string Email) : IRequest<bool>;

internal class SendEmployeeCreatePasswordEmailCommandHandler(
    ILogger<SendEmployeeCreatePasswordEmailCommandHandler> logger,
    ITenantSettingsCacheService tenantSettingsCacheService,
    IUserManagementService userManagementService,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration,
    ISystemUserContextAccessor systemUserContextAccessor,
    ILocalizationService localizationService) : IRequestHandler<SendEmployeeCreatePasswordEmailCommand, bool>
{
    readonly ILogger<SendEmployeeCreatePasswordEmailCommandHandler> logger = logger;
    readonly ITenantSettingsCacheService tenantSettingsCacheService = tenantSettingsCacheService;
    readonly IUserManagementService userManagementService = userManagementService;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;
    readonly ISystemUserContextAccessor systemUserContextAccessor = systemUserContextAccessor;
    readonly ILocalizationService localizationService = localizationService;

    public async Task<bool> Handle(SendEmployeeCreatePasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManagementService.GetUserByEmail(request.Email);

        var emailType = EmailType.EmployeePasswordCreation;
        if (user == null)
        {
            this.logger.LogWarning("User with Email {Email} was not found. {EmailType} was not send",
                request.Email,
                emailType);
            throw new ValidationErrorsException("Email", this.localizationService["ForgotPasswordUserWithEmailNotFound"]);
        }

        var emailTemplate = await this.emailHelperService.GetEmailTemplate(emailType, SupportLanguages.EN.ToString());
        if (emailTemplate == null)
        {
            this.logger.LogWarning("Email Template with Key {EmailType} was not found. {EmailType} was not send",
                emailType, emailType);
            return false;
        }

        var settings = await tenantSettingsCacheService.GetGlobalTenantSettingsAsync(cancellationToken);

        var token = await this.userManagementService.GeneratePasswordResetTokenAsync(request.Email);
        var encodedToken = WebUtility.UrlEncode(token);
        var frontendUrl = configuration.GetSection("Frontend_URL").Value;
        var callbackUrl = $"{frontendUrl}/create-employee-password?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";

        var body = this.emailHelperService.LoadTemplate(emailTemplate.TemplateHtml, new Dictionary<string, string>
        {
            { EmployeePasswordCreation.CreatePasswordUrl, callbackUrl },
            { EmployeePasswordCreation.ClubName, settings.ClubName },
        });

        var subject = this.emailHelperService.LoadTemplate(emailTemplate.Subject, new Dictionary<string, string>
        {
            { EmployeePasswordCreation.ClubName, settings.ClubName },
        });

        var isEmailSendSuccessfully = this.emailSender.SendEmail(new EmailMessage
        {
            To = user.Email!,
            Subject = subject,
            Body = body
        });

        if (string.IsNullOrWhiteSpace(user.TenantId))
        {
            this.logger.LogInformation(
                "Employee create password email history was not saved because user {UserId} has no tenant.",
                user.Id);
        }
        else
        {
            this.systemUserContextAccessor.Set(new EmailHistorySystemUserContext(user.TenantId, user.Id));
            await this.emailHelperService.CreateEmailHistory(new EmailHistory
            {
                TenantId = user.TenantId,
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

        this.logger.LogInformation("Employee Create Password Email was sent to {Email}. Success: {IsEmailSendSuccessfully}",
            request.Email,
            isEmailSendSuccessfully);

        return isEmailSendSuccessfully;
    }

    private sealed class EmailHistorySystemUserContext(string tenantId, string userId) : IUserContext
    {
        public string? TenantId => tenantId;
        public string? UserId => userId;
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => false;
        public bool IsSystem => true;
    }
}
