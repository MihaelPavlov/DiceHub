using DH.Domain.Adapters.Localization;
using DH.Domain.Enums;
using DH.OperationResultCore.Exceptions;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.ChallengeModels.Commands;

public class CreateChallengeDto : IValidableFields
{
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int Attempts { get; set; }
    public int GameId { get; set; }

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (!Enum.IsDefined(typeof(ChallengeRewardPoint), RewardPoints))
            errors.Add(new ValidationError(nameof(RewardPoints), "Invalid reward points value."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
