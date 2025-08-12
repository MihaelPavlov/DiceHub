using DH.Domain.Enums;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.Common;

public class UserSettingsDto : IValidableFields
{
    public int? Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrEmpty(PhoneNumber))
            errors.Add(new ValidationError(nameof(PhoneNumber),
                $"Phone Number is required."));

        if (!Enum.TryParse<SupportLanguages>(Language, out var parsedLanguage))
            errors.Add(new ValidationError(nameof(Language),
                $"{parsedLanguage} is not a valid value."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
