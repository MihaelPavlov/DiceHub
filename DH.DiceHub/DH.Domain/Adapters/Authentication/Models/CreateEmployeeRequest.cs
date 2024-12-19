using DH.Domain.Models;
using System.ComponentModel.DataAnnotations;
using static DH.Domain.Exceptions.ValidationErrorsException;

namespace DH.Domain.Adapters.Authentication.Models;

public class CreateEmployeeRequest : IValidableFields
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrEmpty(Email))
            errors.Add(new ValidationError(nameof(Email),
                "Email is required."));

        if (!new EmailAddressAttribute().IsValid(Email))
            errors.Add(new ValidationError(nameof(Email),
                "Email is invalid."));

        if (string.IsNullOrEmpty(FirstName))
            errors.Add(new ValidationError(nameof(FirstName),
                $"First Name is required."));

        if (string.IsNullOrEmpty(LastName))
            errors.Add(new ValidationError(nameof(LastName),
                $"Last Name is required."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
