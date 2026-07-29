using DH.Domain.Adapters.Localization;
using DH.Domain.Enums;
using System.Text.RegularExpressions;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.Common;

public class TenantApplicationRequest : IValidableFields
{
    private const int MaxEmailLength = 254;
    private const int MaxPhoneNumberLength = 20;
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
    private static readonly Regex PhoneRegex = new(@"^\+?[0-9\s\-]{7,20}$");

    public string ApplicantType { get; set; } = "Venue/Club";
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PublicWebsite { get; set; } = string.Empty;
    public string SocialPage { get; set; } = string.Empty;
    public string DiscordServer { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (!IsEmailVerified)
            errors.Add(new ValidationError(nameof(IsEmailVerified), "Email must be verified before submitting an application."));

        if (string.IsNullOrWhiteSpace(ContactName) || ContactName.Length > 100)
            errors.Add(new ValidationError(nameof(ContactName), "Contact name is required and must be under 100 characters."));

        if (string.IsNullOrWhiteSpace(Email) || Email.Length > MaxEmailLength || !EmailRegex.IsMatch(Email))
            errors.Add(new ValidationError(nameof(Email), "Valid email is required."));

        if (string.IsNullOrWhiteSpace(PhoneNumber) || PhoneNumber.Length > MaxPhoneNumberLength || !PhoneRegex.IsMatch(PhoneNumber))
            errors.Add(new ValidationError(nameof(PhoneNumber), "Valid phone number is required."));

        if (string.IsNullOrWhiteSpace(Address) || Address.Length > 300)
            errors.Add(new ValidationError(nameof(Address), "Address is required and must be under 300 characters."));

        if (string.IsNullOrWhiteSpace(PublicWebsite) && string.IsNullOrWhiteSpace(SocialPage) && string.IsNullOrWhiteSpace(DiscordServer))
            errors.Add(new ValidationError(nameof(PublicWebsite), "Provide at least one public website, social page, or Discord server."));

        if (PhotoUrl.Length > 1000)
            errors.Add(new ValidationError(nameof(PhotoUrl), "Photo URL must be under 1000 characters."));

        validationErrors = errors;
        return !errors.Any();
    }
}

public class TenantApplicationReviewRequest
{
    public string? Note { get; set; }
}

public class TenantApplicationSendEmailCodeRequest
{
    public string Email { get; set; } = string.Empty;
    public string? Language { get; set; }
}

public class TenantApplicationVerifyEmailCodeRequest
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class TenantApplicationDto
{
    public int Id { get; set; }
    public string ApplicantType { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PublicWebsite { get; set; } = string.Empty;
    public string SocialPage { get; set; } = string.Empty;
    public string DiscordServer { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public TenantApplicationStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string? ReviewedByUserId { get; set; }
    public string? ReviewNote { get; set; }
}
