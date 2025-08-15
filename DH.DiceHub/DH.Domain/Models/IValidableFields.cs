using DH.Domain.Adapters.Localization;
using static DH.OperationResultCore.Exceptions.ValidationErrorsException;

namespace DH.Domain.Models;

internal interface IValidableFields
{
    bool FieldsAreValid(out List<ValidationError> validationErrors, ILocalizationService localizationService);
}
