using DH.Domain.Adapters.Localization;
using DH.Domain.Enums;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.Common;

public class UserSettingsDto : IValidableFields
{
    public int? Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;

    public bool InternalUpdate = false;

    public bool FieldsAreValid(out List<ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrEmpty(PhoneNumber))
            errors.Add(new ValidationError(nameof(PhoneNumber), localizationService["TenantSettingsPhoneNumber"]));

        if (!InternalUpdate)
        {
            if (!Enum.TryParse<SupportLanguages>(Language, out var parsedLanguage))
                errors.Add(new ValidationError(nameof(Language), localizationService["UserSettingsLanguage"]));
        }

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
