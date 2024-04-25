namespace DH.Domain.Models;

internal interface IValidableFields
{
    bool FieldsAreValied();
    dynamic ValidateFields();
}
