using System.Text.RegularExpressions;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.Common;

public class PartnerInquiryDto : IValidableFields
{
    private const int MinNameLength = 3;
    private const int MaxNameLength = 100;
    private const int MaxMessageLength = 1000;
    private const int MaxEmailLength = 254;
    private const int MaxPhoneNumberLength = 20;

    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new Regex(
        @"^\+?[0-9\s\-]{7,20}$"); // Allows optional +, numbers, spaces, dashes

    private static readonly Regex NameRegex = new Regex(
        @"^[a-zA-Z\s\-']+$"); // Only letters, space, dash, apostrophe

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length < MinNameLength || Name.Length > MaxNameLength)
        {
            errors.Add(new ValidationError(nameof(Name), $"Name should be between {MinNameLength} and {MaxNameLength} characters."));
        }
        else if (!NameRegex.IsMatch(Name))
        {
            errors.Add(new ValidationError(nameof(Name), "Name contains invalid characters."));
        }

        if (string.IsNullOrWhiteSpace(Email) || Email.Length > MaxEmailLength)
        {
            errors.Add(new ValidationError(nameof(Email), "Email should not be empty or too long."));
        }
        else if (!EmailRegex.IsMatch(Email))
        {
            errors.Add(new ValidationError(nameof(Email), "Invalid email format."));
        }

        if (string.IsNullOrWhiteSpace(Message) || Message.Length > MaxMessageLength)
        {
            errors.Add(new ValidationError(nameof(Message), $"Message must be provided and under {MaxMessageLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber) || PhoneNumber.Length > MaxPhoneNumberLength)
        {
            errors.Add(new ValidationError(nameof(PhoneNumber), "Phone number must be provided and under 20 characters."));
        }
        else if (!PhoneRegex.IsMatch(PhoneNumber))
        {
            errors.Add(new ValidationError(nameof(PhoneNumber), "Invalid phone number format."));
        }

        validationErrors = errors;
        return !errors.Any();
    }
}
