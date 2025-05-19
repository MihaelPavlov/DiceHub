using DH.Domain.Models;
using System.ComponentModel.DataAnnotations;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Adapters.Authentication.Models;

public class UpdateEmployeeRequest : IValidableFields
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrEmpty(Id))
            errors.Add(new ValidationError(nameof(Id),
                "Id was not send."));

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

        if (string.IsNullOrEmpty(PhoneNumber))
            errors.Add(new ValidationError(nameof(PhoneNumber),
                $"Phone Number is required."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
