using DH.Domain.Models;
using System.ComponentModel.DataAnnotations;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Adapters.Authentication.Models;

public class CreateOwnerRequest : IValidableFields
{
    public string Email { get; set; } = string.Empty;
    public string ClubPhoneNumber { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrEmpty(Email))
            errors.Add(new ValidationError(nameof(Email),
                "Email is required."));

        if (!new EmailAddressAttribute().IsValid(Email))
            errors.Add(new ValidationError(nameof(Email),
                "Email is invalid."));

        if (string.IsNullOrEmpty(ClubName))
            errors.Add(new ValidationError(nameof(ClubName),
                $"Club Name is required."));

        if (string.IsNullOrEmpty(ClubPhoneNumber))
            errors.Add(new ValidationError(nameof(ClubPhoneNumber),
                $"Club Phone Number is required."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
