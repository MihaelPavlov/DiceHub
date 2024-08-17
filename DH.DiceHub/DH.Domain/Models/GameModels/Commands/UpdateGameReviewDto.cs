using static DH.Domain.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models.GameModels.Commands;

public class UpdateGameReviewDto : IValidableFields
{
    private const int MinReviewLength = 10;
    private const int MaxReviewLength = 100;

    public int Id { get; set; }
    public string Review { get; set; } = string.Empty;

    public bool FieldsAreValid(out List<ValidationError> validationErrors)
    {
        var errors = new List<ValidationError>();

        if (this.Review.Trim().Length < MinReviewLength)
            errors.Add(new ValidationError(nameof(Review),
                $"Review should be at least {MinReviewLength} characters long."));

        if (this.Review.Trim().Length > MaxReviewLength)
            errors.Add(new ValidationError(nameof(Review),
                $"Review should be max {MaxReviewLength} characters long."));
        validationErrors = errors;

        return !validationErrors.Any();
    }
}
