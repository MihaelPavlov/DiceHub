using DH.Domain.Enums;
using DH.OperationResultCore.Exceptions;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.ChallengeModels.Commands;

public class CreateChallengeDto : IValidableFields
{
    const int MinDescriptionLength = 5;
    const int MaxDescriptionLength = 100;

    public string Description { get; set; } = string.Empty;
    public ChallengeRewardPoint RewardPoints { get; set; }
    public int Attempts { get; set; }
    public ChallengeType Type { get; set; }
    public int GameId { get; set; }

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();
        if (Description.Trim().Length < MinDescriptionLength || Description.Trim().Length > MaxDescriptionLength)
            errors.Add(new ValidationError(nameof(Description), $"Description must be between {MinDescriptionLength} - {MaxDescriptionLength} characters long."));

        if (!Enum.IsDefined(typeof(ChallengeRewardPoint), RewardPoints))
            errors.Add(new ValidationError(nameof(RewardPoints), "Invalid reward points value."));

        validationErrors = errors;

        return !validationErrors.Any();
    }
}
