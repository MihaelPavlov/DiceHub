using DH.Domain.Adapters.Localization;
using DH.Domain.Models;
using DH.OperationResultCore.Exceptions;
using System.Text.RegularExpressions;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Adapters.Authentication.Models;

public class UserRegistrationRequest : IValidableFields
{
    public const int PASSWORD_MIN_LENGTH = 5;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? DeviceToken { get; set; }
    public string? Language { get; set; }

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (!Regex.IsMatch(Email, "^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$"))
            errors.Add(new ValidationError(nameof(Email), localizationService["UserRegistrationValidationInvalidEmail"]));

        if (Password.Length < PASSWORD_MIN_LENGTH)
            errors.Add(new ValidationError(nameof(Password),
                string.Format(localizationService["UserRegistrationValidationPasswordTooShort"], PASSWORD_MIN_LENGTH)));

        if (Password != ConfirmPassword)
            errors.Add(new ValidationError(nameof(ConfirmPassword), localizationService["UserRegistrationValidationPasswordsDoNotMatch"]));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}

public class RegistrationNotifcation
{
    public string Email { get; set; } = string.Empty;
}
