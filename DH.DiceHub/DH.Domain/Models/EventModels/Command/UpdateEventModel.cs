﻿using System.Text.Json.Serialization;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.EventModels.Command;

public class UpdateEventModel : IValidableFields
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("gameId")]
    public int GameId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("maxPeople")]
    public int MaxPeople { get; set; }

    [JsonPropertyName("isCustomImage")]
    public bool IsCustomImage { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        validationErrors = errors;
        return true;
    }
}
