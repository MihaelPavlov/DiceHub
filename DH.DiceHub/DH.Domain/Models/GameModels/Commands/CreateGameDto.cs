using static DH.Domain.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.GameModels.Commands;

public class CreateGameDto : IValidableFields
{
    private const int MinNameLength = 3;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MinAge { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int AveragePlaytime { get; set; }
    public int Likes { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (MaxPlayers < MinPlayers)
            errors.Add(new ValidationError(nameof(MaxPlayers),
                "Max players cannot be less than the min players."));

        if (Name.Trim().Length < MinNameLength)
            errors.Add(new ValidationError(nameof(Name),
                $"Name should be at least {MinNameLength} characters long."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
