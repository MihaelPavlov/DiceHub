using DH.Domain.Adapters.Localization;
using DH.Domain.Enums;
using DH.OperationResultCore.Exceptions;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.ChallengeModels.Commands;

public class UpdateUniversalChallengeDto : IValidableFields
{
    public int Id { get; set; }
    public int Attempts { get; set; }
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int? MinValue { get; set; }

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors, ILocalizationService localizationService)
    {
        var errors = new List<ValidationError>();

        if (Id <= 0)
            errors.Add(new ValidationError(nameof(Id), localizationService["UpdateUniversalChallengeIdValidation"]));

        if (!Enum.IsDefined(typeof(ChallengeRewardPoint), RewardPoints))
            errors.Add(new ValidationError(nameof(RewardPoints), localizationService["CreateChallengeRewardPointsValidation"]));

        if (Attempts <= 0)
            errors.Add(new ValidationError(nameof(Attempts), localizationService["UpdateUniversalChallengeAttemptsValidation"]));

        if (MinValue.HasValue && MinValue < 0)
            errors.Add(new ValidationError(nameof(MinValue), localizationService["UpdateUniversalChallengeMinValueValidation"]));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
