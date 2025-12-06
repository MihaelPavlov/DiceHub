using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using DH.Domain.Services;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DH.Application.Common.Commands;

public record CreatePartnerInquiriesCommand(PartnerInquiryDto PartnerInquiry) : IRequest;

internal class CreatePartnerInquiriesCommandHandle(
    ILogger<CreatePartnerInquiriesCommand> logger,
    IRepository<PartnerInquiry> repository,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IConfiguration configuration,
    ILocalizationService localizer) : IRequestHandler<CreatePartnerInquiriesCommand>
{
    readonly ILogger<CreatePartnerInquiriesCommand> logger = logger;
    readonly IRepository<PartnerInquiry> repository = repository;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;
    readonly ILocalizationService localizer = localizer;

    public async Task Handle(CreatePartnerInquiriesCommand request, CancellationToken cancellationToken)
    {
        if (!request.PartnerInquiry.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        try
        {
            await this.repository.AddAsync(new PartnerInquiry
            {
                Name = request.PartnerInquiry.Name,
                Email = request.PartnerInquiry.Email,
                Message = request.PartnerInquiry.Message,
                PhoneNumber = request.PartnerInquiry.PhoneNumber
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Partner Inquiry was not saved successfully. Starting Processing Email Notification...");
        }

        var emailType = EmailType.PartnerInquiryRequest;

        var emailTemplate = await this.emailHelperService.GetEmailTemplate(emailType, SupportLanguages.EN.ToString());
        if (emailTemplate == null)
        {
            this.logger.LogWarning("Email Template with Key {EmailType} was not found. {EmailType} was not send",
                emailType, emailType);
            return;
        }

        var supportEmail = configuration.GetSection("SupportEmail").Value ?? "dicehubapp@gmail.com";

        var body = this.emailHelperService.LoadTemplate(emailTemplate.TemplateHtml, new Dictionary<string, string>
        {
            { PartnerInquiryRequest.Name, request.PartnerInquiry.Name },
            { PartnerInquiryRequest.Email,  request.PartnerInquiry.Email },
            { PartnerInquiryRequest.Message, request.PartnerInquiry.Message },
            { PartnerInquiryRequest.PhoneNumber, request.PartnerInquiry.PhoneNumber },
        });

        var isEmailSendSuccessfully = this.emailSender.SendEmail(new EmailMessage
        {
            To = supportEmail,
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
            To = supportEmail,
            UserId = "support",
        });
    }
}
