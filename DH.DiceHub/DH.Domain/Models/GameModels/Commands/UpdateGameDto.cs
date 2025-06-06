﻿using DH.Domain.Enums;
using System.Text.Json.Serialization;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.GameModels.Commands;

public class UpdateGameDto : IValidableFields
{
    private const int MinNameLength = 3;

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("minAge")]
    public int MinAge { get; set; }

    [JsonPropertyName("minPlayers")]
    public int MinPlayers { get; set; }

    [JsonPropertyName("maxPlayers")]
    public int MaxPlayers { get; set; }

    [JsonPropertyName("averagePlaytime")]
    public GameAveragePlaytime AveragePlaytime { get; set; }

    [JsonPropertyName("imageId")]
    public int? ImageId { get; set; }

    [JsonPropertyName("userId")]
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
