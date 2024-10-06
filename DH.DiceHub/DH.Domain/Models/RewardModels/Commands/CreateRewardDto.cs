using DH.Domain.Enums;
using DH.Domain.Exceptions;
using System.Text.Json.Serialization;
using static DH.Domain.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.RewardModels.Commands;

public class CreateRewardDto : IValidableFields
{
    const int MinNameLength = 3;
    const int MinRequiredPoints = 3;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("requiredPoints")]
    public RewardRequiredPoint RequiredPoints { get; set; }

    [JsonPropertyName("level")]
    public RewardLevel Level { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (Name.Trim().Length < MinNameLength)
            errors.Add(new ValidationError(nameof(Name),
                $"Name should be at least {MinNameLength} characters long."));

        if (!Enum.IsDefined(typeof(RewardRequiredPoint), RequiredPoints))
            errors.Add(new ValidationError(nameof(RequiredPoints), "Invalid reward points value."));



        validationErrors = errors;

        return !validationErrors.Any();
    }
}
