using DH.Domain.Adapters.Localization;
using DH.Domain.Models;
using DH.OperationResultCore.Exceptions;
using System.Text.RegularExpressions;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Adapters.Authentication.Models;

public class UserRegistrationRequest : IValidableFields
{
    const int PASSWORD_MIN_LENGTH = 5;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? DeviceToken { get; set; }

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (!Regex.IsMatch(Email, "^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$"))
            errors.Add(new ValidationError(nameof(Email), "Email is invalid."));

        if (Password.Length < PASSWORD_MIN_LENGTH)
            errors.Add(new ValidationError(nameof(Password), $"Passwords shouldn't be less then {PASSWORD_MIN_LENGTH} characters."));

        if (Password != ConfirmPassword)
            errors.Add(new ValidationError(nameof(ConfirmPassword), $"Passwords should match."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}

public class RegistrationNotifcation
{
    public string Email { get; set; } = string.Empty;
}
