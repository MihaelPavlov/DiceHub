using DH.Domain.Exceptions;

using static DH.Domain.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.SpaceManagementModels.Commands;

public class CreateSpaceTableDto : IValidableFields
{
    public string Name { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public string Password { get; set; } = string.Empty;
    public int GameId { get; set; }

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
