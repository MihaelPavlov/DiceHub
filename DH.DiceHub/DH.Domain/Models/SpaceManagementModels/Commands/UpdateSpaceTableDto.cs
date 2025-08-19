using DH.Domain.Adapters.Localization;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.SpaceManagementModels.Commands;

public class UpdateSpaceTableDto : IValidableFields
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public string Password { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrEmpty(Name))
            errors.Add(new ValidationError(nameof(Name), localizationService["RoomValidationNameRequired"]));

        if (MaxPeople <= 1)
            errors.Add(new ValidationError(nameof(MaxPeople), localizationService["RoomValidationMaxPeople"]));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
