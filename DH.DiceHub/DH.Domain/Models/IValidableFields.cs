namespace DH.Domain.Models;

internal interface IValidableFields
{
    bool FieldsAreValid(out IReadOnlyCollection<string> validationErrors);
}
