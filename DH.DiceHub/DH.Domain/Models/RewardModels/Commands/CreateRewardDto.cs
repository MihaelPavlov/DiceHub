using DH.Domain.Adapters.Localization;
using DH.Domain.Enums;
using System.Text.Json.Serialization;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.RewardModels.Commands;

public class CreateRewardDto : IValidableFields
{
    const int MinNameLength = 3;

    [JsonPropertyName("name_EN")]
    public string Name_EN { get; set; } = string.Empty;

    [JsonPropertyName("name_BG")]
    public string Name_BG { get; set; } = string.Empty;

    [JsonPropertyName("cashEquivalent")]
    public decimal CashEquivalent { get; set; }

    [JsonPropertyName("description_EN")]
    public string Description_EN { get; set; } = string.Empty;

    [JsonPropertyName("description_BG")]
    public string Description_BG { get; set; } = string.Empty;

    [JsonPropertyName("requiredPoints")]
    public RewardRequiredPoint RequiredPoints { get; set; }

    [JsonPropertyName("level")]
    public RewardLevel Level { get; set; }

    public bool FieldsAreValid(out List<ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (Name_EN.Trim().Length < MinNameLength)
            errors.Add(new ValidationError(nameof(Name_EN), string.Format(localizationService["RewardNameValidation_En"], MinNameLength)));

        if (Name_BG.Trim().Length < MinNameLength)
            errors.Add(new ValidationError(nameof(Name_BG), string.Format(localizationService["RewardNameValidation_Bg"], MinNameLength)));

        if (!Enum.IsDefined(typeof(RewardRequiredPoint), RequiredPoints))
            errors.Add(new ValidationError(nameof(RequiredPoints), localizationService["RewardRequiredPointsValidation"]));

        if (CashEquivalent < 0 && CashEquivalent >= 1000)
            errors.Add(new ValidationError(nameof(CashEquivalent), localizationService["RewardCashEquivalentValidation"]));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
