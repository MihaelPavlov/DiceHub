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

    // TODO: return a collection of validation messages here and throw the exception after checking the FieldsAreValid in the code
    public bool FieldsAreValid(out IReadOnlyCollection<string> validationErrors)
    {
        var errors = new List<string>();

        if (MaxPlayers < MinPlayers)
        {
            errors.Add("Max players cannot be less than the min players.");
        }

        if (Name.Trim().Length < MinNameLength)
        {
            errors.Add($"Name should be at least {MinNameLength} characters long.");
        }

        if (string.IsNullOrWhiteSpace(UserId))
        {
            errors.Add("UserId is not provided.");
        }
        validationErrors = errors;

        return !validationErrors.Any();
    }
}
