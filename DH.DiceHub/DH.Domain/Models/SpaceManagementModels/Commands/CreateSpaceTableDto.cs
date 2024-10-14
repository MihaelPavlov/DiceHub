using DH.Domain.Exceptions;

using static DH.Domain.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.SpaceManagementModels.Commands;

public class CreateSpaceTableDto : IValidableFields
{
    public string Name { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public string Password { get; set; } = string.Empty;
    public int GameId { get; set; }
    public bool IsSoloModeActive { get; set; }

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (IsSoloModeActive)
        {
            validationErrors = errors;
            return true;
        }

        if (string.IsNullOrEmpty(Name))
            errors.Add(new ValidationError(nameof(Name), "Name should not be empty."));

        if (MaxPeople <= 1)
            errors.Add(new ValidationError(nameof(MaxPeople), "Max People should not be less or equal to 1."));

        if (GameId <= 0)
            errors.Add(new ValidationError(nameof(GameId), "Scanned game is invalid. Contact the staff"));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
