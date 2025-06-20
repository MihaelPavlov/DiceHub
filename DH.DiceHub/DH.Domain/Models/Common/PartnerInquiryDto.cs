using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.Common;

public class PartnerInquiryDto : IValidableFields
{
    const int MinNameLength = 3;

    public string Name { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public string PhoneNumber { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (Name.Trim().Length < MinNameLength)
            errors.Add(new ValidationError(nameof(Name),
                $"Name should be at least {MinNameLength} characters long."));

        if (string.IsNullOrEmpty(Email))
            errors.Add(new ValidationError(nameof(Email),
                $"Email should not be empty."));

        if (string.IsNullOrEmpty(Message))
            errors.Add(new ValidationError(nameof(Message),
                $"Message should not be empty."));

        if (string.IsNullOrEmpty(PhoneNumber))
            errors.Add(new ValidationError(nameof(PhoneNumber),
                $"Phone Number should not be empty."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}