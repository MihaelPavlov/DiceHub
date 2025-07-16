using DH.OperationResultCore.Exceptions;

namespace DH.Domain.Models.ChallengeModels.Commands;

public class SaveCustomPeriodDto : IValidableFields
{
    public List<SaveCustomPeriodRewardDto> Rewards { get; set; } = [];
    public List<SaveCustomPeriodChallengeDto> Challenges { get; set; } = [];

    public bool FieldsAreValid(out List<ValidationErrorsException.ValidationError> validationErrors)
    {
        var errors = new List<ValidationErrorsException.ValidationError>();

        // --- Validate Challenges ---
        for (int i = 0; i < Challenges.Count; i++)
        {
            var challenge = Challenges[i];

            if (challenge.Attempts <= 0)
            {
                errors.Add(new ValidationErrorsException.ValidationError(
                    $"Challenges[{i}].Attempts",
                    "Attempts must be greater than zero."));
            }

            if (challenge.Points <= 0)
            {
                errors.Add(new ValidationErrorsException.ValidationError(
                    $"Challenges[{i}].Points",
                    "Points must be greater than zero."));
            }
        }

        // --- Validate Rewards ---

        for (int i = 0; i < Rewards.Count - 1; i++)
        {
            if (Rewards[i].RequiredPoints > Rewards[i + 1].RequiredPoints)
            {
                errors.Add(new ValidationErrorsException.ValidationError(
                    $"Rewards[{i}].RequiredPoints",
                    "Rewards must be ordered by Required Points in ascending order."));
                break; // one misorder is enough to fail the sort rule
            }
        }

        // --- Validate Totals Match ---
        int totalChallengePoints = Challenges.Sum(c => c.Points);
        int totalRewardRequiredPoints = Rewards.Max(r => r.RequiredPoints);

        if (totalRewardRequiredPoints != totalChallengePoints)
        {
            errors.Add(new ValidationErrorsException.ValidationError(
                "Rewards",
                $"Total reward required points ({totalRewardRequiredPoints}) must equal total challenge points ({totalChallengePoints})."));
        }

        validationErrors = errors;
        return !validationErrors.Any();
    }
}

public class SaveCustomPeriodRewardDto
{
    public int? Id { get; set; }
    public int SelectedReward { get; set; }
    public int RequiredPoints { get; set; }
}

public class SaveCustomPeriodChallengeDto
{
    public int? Id { get; set; } 
    public int SelectedGame { get; set; }
    public int Attempts { get; set; }
    public int Points { get; set; }
}
