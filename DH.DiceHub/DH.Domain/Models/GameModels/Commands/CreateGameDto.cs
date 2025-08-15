
using DH.Domain.Adapters.Localization;
using DH.Domain.Enums;
using System.Text.Json.Serialization;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.GameModels.Commands;

public class CreateGameDto : IValidableFields
{
    const int MinNameLength = 3;

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description_EN")]
    public string Description_EN { get; set; } = string.Empty;

    [JsonPropertyName("description_BG")]
    public string Description_BG { get; set; } = string.Empty;

    [JsonPropertyName("minAge")]
    public int MinAge { get; set; }

    [JsonPropertyName("minPlayers")]
    public int MinPlayers { get; set; }

    [JsonPropertyName("maxPlayers")]
    public int MaxPlayers { get; set; }

    [JsonPropertyName("averagePlaytime")]
    public GameAveragePlaytime AveragePlaytime { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (MaxPlayers < MinPlayers)
            errors.Add(new ValidationError(nameof(MaxPlayers), localizationService["MaxPlayersCannotBeLessThanMinPlayers"]));

        if (Name.Trim().Length < MinNameLength)
            errors.Add(new ValidationError(nameof(Name), localizationService["NameShouldBeAtLeast3CharactersLong"]));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
