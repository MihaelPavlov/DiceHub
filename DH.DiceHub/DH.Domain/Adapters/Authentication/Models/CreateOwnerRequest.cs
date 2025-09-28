using DH.Domain.Adapters.Localization;
using DH.Domain.Models;
using System.ComponentModel.DataAnnotations;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Adapters.Authentication.Models;

public class CreateOwnerRequest : IValidableFields
{
    public string Email { get; set; } = string.Empty;
    public string ClubPhoneNumber { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrEmpty(Email))
            errors.Add(new ValidationError(nameof(Email),
                localizationService["OwnerValidationEmailRequired"]));

        if (!new EmailAddressAttribute().IsValid(Email))
            errors.Add(new ValidationError(nameof(Email),
                localizationService["OwnerValidationEmailInvalid"]));

        if (string.IsNullOrEmpty(ClubName))
            errors.Add(new ValidationError(nameof(ClubName),
                localizationService["OwnerClubNameRequired"]));

        if (string.IsNullOrEmpty(ClubPhoneNumber))
            errors.Add(new ValidationError(nameof(ClubPhoneNumber),
                localizationService["OwnerClubPhoneNumberRequired"]));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
