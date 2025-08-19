using DH.Domain.Adapters.Localization;
using DH.OperationResultCore.Exceptions;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.SpaceManagementModels.Commands;

public class CreateSpaceTableDto : IValidableFields
{
    public string Name { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public string Password { get; set; } = string.Empty;
    public int GameId { get; set; }
    public bool IsSoloModeActive { get; set; }

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (IsSoloModeActive)
        {
            validationErrors = errors;
            return true;
        }

        if (string.IsNullOrEmpty(Name))
            errors.Add(new ValidationError(nameof(Name), localizationService["RoomValidationNameRequired"]));

        if (MaxPeople <= 1)
            errors.Add(new ValidationError(nameof(MaxPeople), localizationService["RoomValidationMaxPeople"]));

        if (GameId <= 0)
            errors.Add(new ValidationError(nameof(GameId), localizationService["RoomValidationInvalidGame"]));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
