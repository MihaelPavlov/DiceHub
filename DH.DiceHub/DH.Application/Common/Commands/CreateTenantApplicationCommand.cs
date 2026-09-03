using DH.Domain.Adapters.Authentication;
using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Adapters.FileManager;
using DH.Domain.Adapters.Localization;
using DH.Domain.Entities;
using DH.Domain.Models.Common;
using DH.Domain.Repositories;
using DH.OperationResultCore.Exceptions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DH.Application.Common.Commands;

public record CreateTenantApplicationCommand(
    TenantApplicationRequest Application,
    string? LogoFileName = null,
    MemoryStream? LogoStream = null) : IRequest<int>;

internal class CreateTenantApplicationCommandHandler(
    IRepository<TenantApplication> repository,
    ISystemUserContextAccessor systemUserContextAccessor,
    ILocalizationService localizer,
    IFileManagerClient fileManagerClient,
    IEmailSender emailSender,
    IConfiguration configuration,
    ILogger<CreateTenantApplicationCommandHandler> logger,
    IMemoryCache memoryCache) : IRequestHandler<CreateTenantApplicationCommand, int>
{
    const string DefaultNotificationEmail = "m.pavlov1405@gmail.com";

    readonly IRepository<TenantApplication> repository = repository;
    readonly ISystemUserContextAccessor systemUserContextAccessor = systemUserContextAccessor;
    readonly ILocalizationService localizer = localizer;
    readonly IFileManagerClient fileManagerClient = fileManagerClient;
    readonly IEmailSender emailSender = emailSender;
    readonly IConfiguration configuration = configuration;
    readonly ILogger<CreateTenantApplicationCommandHandler> logger = logger;
    readonly IMemoryCache memoryCache = memoryCache;

    public async Task<int> Handle(CreateTenantApplicationCommand request, CancellationToken cancellationToken)
    {
        if (!request.Application.FieldsAreValid(out var validationErrors, localizer))
            throw new ValidationErrorsException(validationErrors);

        var normalizedEmail = request.Application.Email.Trim().ToLowerInvariant();
        if (!this.memoryCache.TryGetValue<bool>(TenantApplicationEmailVerificationCache.BuildVerifiedKey(normalizedEmail), out var isEmailVerified) || !isEmailVerified)
            throw new ValidationErrorsException(nameof(request.Application.Email), "Email verification has expired. Verify the email again.");

        var photoUrl = request.Application.PhotoUrl;
        if (request.LogoStream is not null && !string.IsNullOrWhiteSpace(request.LogoFileName))
        {
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.LogoFileName)}";
            photoUrl = await fileManagerClient.UploadFileAsync(
                FileManagerFolders.TenantApplications.ToString(), uniqueFileName, request.LogoStream.ToArray());
        }

        systemUserContextAccessor.Set(new TenantApplicationSystemUserContext());

        var application = new TenantApplication
        {
            ApplicantType = request.Application.ApplicantType,
            ContactName = request.Application.ContactName,
            Email = normalizedEmail,
            PhoneNumber = request.Application.PhoneNumber,
            IsEmailVerified = request.Application.IsEmailVerified,
            IsPhoneVerified = request.Application.IsPhoneVerified,
            Address = request.Application.Address,
            PublicWebsite = request.Application.PublicWebsite,
            SocialPage = request.Application.SocialPage,
            DiscordServer = request.Application.DiscordServer,
            PhotoUrl = photoUrl,
            CreatedDate = DateTime.UtcNow,
        };

        var result = await repository.AddAsync(application, cancellationToken);

        this.memoryCache.Remove(TenantApplicationEmailVerificationCache.BuildVerifiedKey(normalizedEmail));

        NotifyNewTenantApplicant(application);

        return result.Id;
    }

    /// <summary>
    /// Sends a plain notification email with every submitted detail to the configured
    /// recipient (see the "TenantApplicationNotificationEmail" setting). A mail failure
    /// must never fail the application submission, so everything is wrapped and logged.
    /// </summary>
    void NotifyNewTenantApplicant(TenantApplication application)
    {
        try
        {
            var to = this.configuration.GetSection("TenantApplicationNotificationEmail").Value;
            if (string.IsNullOrWhiteSpace(to))
                to = DefaultNotificationEmail;

            var sent = this.emailSender.SendEmail(new EmailMessage
            {
                To = to,
                Subject = $"New tenant applicant: {application.ContactName}",
                Body = BuildApplicantEmailBody(application),
            });

            if (!sent)
                this.logger.LogWarning("New tenant applicant notification email was not sent for application {Id}.", application.Id);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to send new tenant applicant notification email for application {Id}.", application.Id);
        }
    }

    static string BuildApplicantEmailBody(TenantApplication application)
    {
        static string Cell(string label, string? value) =>
            $"<tr><td style=\"padding:6px 12px;font-weight:bold;\">{WebUtility.HtmlEncode(label)}</td>" +
            $"<td style=\"padding:6px 12px;\">{WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(value) ? "-" : value)}</td></tr>";

        return $$"""
            <!DOCTYPE html>
            <html lang="en">
              <body style="font-family:Arial,sans-serif;color:#20232a;">
                <h2>New tenant applicant</h2>
                <table cellpadding="0" cellspacing="0" role="presentation" style="border-collapse:collapse;">
                  {{Cell("Application ID", application.Id.ToString())}}
                  {{Cell("Applicant type", application.ApplicantType)}}
                  {{Cell("Contact name", application.ContactName)}}
                  {{Cell("Email", application.Email)}}
                  {{Cell("Email verified", application.IsEmailVerified ? "Yes" : "No")}}
                  {{Cell("Phone number", application.PhoneNumber)}}
                  {{Cell("Phone verified", application.IsPhoneVerified ? "Yes" : "No")}}
                  {{Cell("Address", application.Address)}}
                  {{Cell("Public website", application.PublicWebsite)}}
                  {{Cell("Social page", application.SocialPage)}}
                  {{Cell("Discord server", application.DiscordServer)}}
                  {{Cell("Logo URL", application.PhotoUrl)}}
                  {{Cell("Status", application.Status.ToString())}}
                  {{Cell("Submitted (UTC)", application.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"))}}
                </table>
              </body>
            </html>
            """;
    }

    private sealed class TenantApplicationSystemUserContext : IUserContext
    {
        public string? TenantId => null;
        public string? UserId => "tenant-application";
        public int? RoleKey => null;
        public string? TimeZone => "UTC";
        public string? Language => "en";
        public bool IsAuthenticated => false;
        public bool IsSystem => true;
    }
}
