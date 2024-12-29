using DH.Domain.Enums;
using System.Text.Json.Serialization;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.RewardModels.Commands;

public class UpdateRewardDto : IValidableFields
{
    const int MinNameLength = 3;
    const int MinRequiredPoints = 3;

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("requiredPoints")]
    public int RequiredPoints { get; set; }

    [JsonPropertyName("level")]
    public RewardLevel Level { get; set; }

    [JsonPropertyName("imageId")]
    public int? ImageId { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();
        if (Id == 0)
            errors.Add(new ValidationError(nameof(Id),
                $"Id is required."));

        if (Name.Trim().Length < MinNameLength)
            errors.Add(new ValidationError(nameof(Name),
                $"Name should be at least {MinNameLength} characters long."));

        if (RequiredPoints < MinRequiredPoints)
            errors.Add(new ValidationError(nameof(RequiredPoints),
                $"RequiredPoints should be bigger then {MinRequiredPoints}."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
