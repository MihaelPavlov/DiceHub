using DH.Domain.Adapters.Authentication.Services;
using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Entities;
using DH.Domain.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DH.Application.Emails.Commands;

public record SendRegistrationEmailConfirmationCommand(string UserId) : IRequest;

internal class SendRegistrationEmailConfirmationCommandHandler(
    ILogger<SendRegistrationEmailConfirmationCommandHandler> logger,
    IUserService userService,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration) : IRequestHandler<SendRegistrationEmailConfirmationCommand>
{
    readonly ILogger<SendRegistrationEmailConfirmationCommandHandler> logger = logger;
    readonly IUserService userService = userService;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;

    public async Task Handle(SendRegistrationEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userService.GetUserById(request.UserId, cancellationToken);

        if (user == null)
        {
            this.logger.LogWarning("User with ID {UserId} was not found. {EmailType} was not send",
                request.UserId,
                EmailType.RegistrationEmailConfirmation.ToString());
            return;
        }

        var emailTemplate = await this.emailHelperService.GetEmailTemplate(EmailType.RegistrationEmailConfirmation);
        if (emailTemplate == null)
        {
            this.logger.LogWarning("Email Template with Key {EmailType} was not found. {EmailType} was not send",
                EmailType.RegistrationEmailConfirmation.ToString(),
                EmailType.RegistrationEmailConfirmation.ToString());
            return;
        }

        var token = await this.userService.GenerateEmailConfirmationTokenAsync(request.UserId);
        var encodedToken = WebUtility.UrlEncode(token);
        var frontendUrl = configuration.GetSection("Frontend_URL").Value;
        var callbackUrl = $"{frontendUrl}/confirm-email?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";

        var body = this.emailHelperService.LoadTemplate(emailTemplate.TemplateHtml, new Dictionary<string, string>
        {
            { RegistrationEmailTemplateKeys.CallbackUrl, callbackUrl },
            { RegistrationEmailTemplateKeys.ClubName, "DiceHub"  } //TODO: Add club name to global tenant setting
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
            UserId = request.UserId,
        });
    }
}
