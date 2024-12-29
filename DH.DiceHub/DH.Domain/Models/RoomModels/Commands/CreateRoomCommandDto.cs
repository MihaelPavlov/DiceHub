using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.RoomModels.Commands;

public class CreateRoomCommandDto : IValidableFields
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int MaxParticipants { get; set; }
    public int GameId { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        validationErrors = errors;

        return !validationErrors.Any();
    }
}