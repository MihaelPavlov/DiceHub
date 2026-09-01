using DH.Domain.Adapters.Email;
using DH.Domain.Adapters.EmailSender;
using DH.Domain.Enums;
using DH.Domain.Models.Common;
using DH.Domain.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Security.Cryptography;

namespace DH.Application.Common.Commands;

public record SendTenantApplicationEmailVerificationCodeCommand(TenantApplicationSendEmailCodeRequest Request) : IRequest<bool>;

internal class SendTenantApplicationEmailVerificationCodeCommandHandler(
    ILogger<SendTenantApplicationEmailVerificationCodeCommandHandler> logger,
    IEmailHelperService emailHelperService,
    IEmailSender emailSender,
    IMemoryCache memoryCache) : IRequestHandler<SendTenantApplicationEmailVerificationCodeCommand, bool>
{
    const int CodeExpirationMinutes = 10;

    readonly ILogger<SendTenantApplicationEmailVerificationCodeCommandHandler> logger = logger;
    readonly IEmailHelperService emailHelperService = emailHelperService;
    readonly IEmailSender emailSender = emailSender;
    readonly IMemoryCache memoryCache = memoryCache;

    public async Task<bool> Handle(SendTenantApplicationEmailVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Request.Email);
        if (!IsValidEmail(email))
            return false;

        var code = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
        var emailType = EmailType.TenantApplicationEmailVerification;
        var language = string.IsNullOrWhiteSpace(request.Request.Language)
            ? SupportLanguages.EN.ToString()
            : request.Request.Language;

        var emailTemplate = await this.emailHelperService.GetEmailTemplate(emailType, language);
        var subject = emailTemplate?.Subject ?? "DiceHub venue application verification code";
        var body = this.emailHelperService.LoadTemplate(
            emailTemplate?.TemplateHtml ?? GetFallbackTemplate(),
            new Dictionary<string, string>
            {
                { TenantApplicationEmailVerification.VerificationCode, code },
            });

        var isEmailSendSuccessfully = this.emailSender.SendEmail(new EmailMessage
        {
            To = email,
            Subject = subject,
            Body = body,
        });

        this.logger.LogInformation(
            "Tenant application email verification history was not saved because the request has no tenant yet. Email: {Email}",
            email);

        if (!isEmailSendSuccessfully)
        {
            this.logger.LogWarning("Tenant application email verification code was not sent to {Email}.", email);
            return false;
        }

        this.memoryCache.Set(
            TenantApplicationEmailVerificationCache.BuildKey(email),
            new TenantApplicationEmailVerificationCache.Entry(HashCode(code), 0),
            TimeSpan.FromMinutes(CodeExpirationMinutes));

        return true;
    }

    static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    static string HashCode(string code)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(code));
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
                  <table width="500" cellpadding="0" cellspacing="0" role="presentation" style="background-color:#20232a;padding:20px;border-radius:8px;">
                    <tr>
                      <td align="center" style="font-size:22px;font-weight:bold;padding-bottom:20px;">
                        DiceHub venue application
                      </td>
                    </tr>
                    <tr>
                      <td style="font-size:16px;line-height:1.6;">
                        <p>Your verification code is:</p>
                        <p style="font-size:32px;font-weight:bold;letter-spacing:6px;color:#75a0ff;">{{VerificationCode}}</p>
                        <p>This code expires in 10 minutes.</p>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>
          </body>
        </html>
        """;
}

internal static class TenantApplicationEmailVerificationCache
{
    public sealed record Entry(string CodeHash, int Attempts);

    public static string BuildKey(string email) => $"tenant-application:email-verification:{email.Trim().ToLowerInvariant()}";

    public static string BuildVerifiedKey(string email) => $"tenant-application:email-verified:{email.Trim().ToLowerInvariant()}";
}
