using DH.OperationResultCore.Utility;
using System.Net;

namespace DH.OperationResultCore.Extension;

public static class OperationResultExtension
{
    /// <summary>
    /// Appends error messages from one <typeparamref name="TOriginal"/> to another <typeparamref name="TOther"/>.
    /// </summary>
    /// <typeparam name="TOriginal">A type that inherits from <see cref="OperationResult"/>.</typeparam>
    /// <typeparam name="TOther">A type that inherits from <see cref="OperationResult"/>.</typeparam>
    /// <param name="originalOperationResult">The <see cref="OperationResult"/> to append to.</param>
    /// <param name="otherOperationResult">The <see cref="OperationResult"/> to append from.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static TOriginal AppendErrorMessage<TOriginal, TOther>(this TOriginal originalOperationResult, TOther otherOperationResult)
           where TOriginal : OperationResult
           where TOther : OperationResult
    {
        if (originalOperationResult is null)
            throw new ArgumentNullException(nameof(originalOperationResult));

        if (otherOperationResult is null)
            return originalOperationResult;

        originalOperationResult.Success = false;
        originalOperationResult.InitialException = otherOperationResult.InitialException;

        return originalOperationResult;
    }

    /// <summary>
    /// Appends error messages from one <typeparamref name="TOriginal"/> to another <typeparamref name="TOther"/>.
    /// </summary>
    /// <param name="originalOperationResult">The <see cref="OperationResult"/> to append to.</param>
    /// <param name="otherOperationResult">The <see cref="OperationResult"/> to append from.</param>
    /// <typeparam name="TOriginal">A type that inherits from <see cref="OperationResult"/>.</typeparam>
    /// <typeparam name="TOther">A type that inherits from <see cref="OperationResult"/>.</typeparam>
    /// <returns>The original <see cref="OperationResult"/> with the appended messages from the other <typeparamref name="TOther"/>.</returns>
    public static TOriginal MergeErrors<TOriginal, TOther>(this TOriginal originalOperationResult, TOther otherOperationResult)
    where TOriginal : OperationResult
    where TOther : OperationResult
    {
        if (originalOperationResult is null)
            throw new ArgumentNullException(nameof(originalOperationResult));

        if (otherOperationResult is null)
            return originalOperationResult;

        // Adding the error message without logging (presuming that there is already a log message).
        if (otherOperationResult.ValidationErrors.Any())
        {
            foreach (var (key, value) in otherOperationResult.ValidationErrors)
            {
                //TODO: Additional Validation if the key already exist
                originalOperationResult.ValidationErrors.Add(key, value);
            }
        }

        if (otherOperationResult.InitialException != null)
            originalOperationResult.InitialException = otherOperationResult.InitialException;

        return originalOperationResult;
    }

    /// <summary>
    /// Use this method in the case when we want to stop the request flow.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="operationResult"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static T ReturnWithErrorMessage<T>(this T operationResult, IError error)
    where T : OperationResult
    {
        operationResult.Success = false;
        operationResult.InitialException = error;

        return operationResult;
    }

    /// <summary>
    /// Method which is return the current instance of the OperationResult<T>.
    /// And it's give us option to pass a initial exception.
    /// </summary>
    /// <param name="error">Optional error which is initializing the InitialException in the current instance.</param>
    public static OperationResult<T> ReturnWithErrorMessage<T>(this OperationResult<T> operationResult, IError? error = null)
    where T : class
    {
        if (error != null)
        {
            operationResult.Success = false;
            operationResult.InitialException = error;
        }

        return operationResult;
    }

    #region Not Found Exception

    /// <summary>
    /// Updates the <see cref="OperationResult"/> to indicate a failure due to a not found exception.
    /// </summary>
    /// <param name="operationResult">The current <see cref="OperationResult"/> instance.</param>
    /// <param name="message">Optional. A custom message describing the exception. Defaults to "Entity was not found."</param>
    /// <returns>The updated <see cref="OperationResult"/> with the not found exception details.</returns>
    public static OperationResult ReturnWithNotFoundException(this OperationResult operationResult, string? message = null)
    {
        operationResult.Success = false;
        operationResult.InitialException = new IError(HttpStatusCode.NotFound.ToString(), (int)HttpStatusCode.NotFound, message ?? "Entity was not found.");

        return operationResult;
    }

    /// <summary>
    /// Updates the <see cref="OperationResult"/> to indicate a failure due to a specific entity not being found.
    /// </summary>
    /// <param name="operationResult">The current <see cref="OperationResult"/> instance.</param>
    /// <param name="entityName">The name of the entity that was not found.</param>
    /// <param name="key">The key or identifier of the missing entity.</param>
    /// <returns>The updated <see cref="OperationResult"/> with the not found exception details.</returns>
    public static OperationResult ReturnWithNotFoundException(this OperationResult operationResult, string entityName, object key)
    {
        operationResult.Success = false;
        operationResult.InitialException = new IError(HttpStatusCode.NotFound.ToString(), (int)HttpStatusCode.NotFound, $"Entity {entityName} ({key}) was not found.");

        return operationResult;
    }

    /// <summary>
    /// Updates the <see cref="OperationResult{T}"/> to indicate a failure due to a specific entity not being found.
    /// </summary>
    /// <typeparam name="T">The type of the result data.</typeparam>
    /// <param name="operationResult">The current <see cref="OperationResult{T}"/> instance.</param>
    /// <param name="entityName">The name of the entity that was not found.</param>
    /// <param name="key">The key or identifier of the missing entity.</param>
    /// <returns>The updated <see cref="OperationResult{T}"/> with the not found exception details.</returns>
    public static OperationResult<T> ReturnWithNotFoundException<T>(this OperationResult<T> operationResult, string entityName, object key)
    {
        operationResult.Success = false;
        operationResult.InitialException = new IError(HttpStatusCode.NotFound.ToString(), (int)HttpStatusCode.NotFound, $"Entity {entityName} ({key}) was not found.");

        return operationResult;
    }

    /// <summary>
    /// Updates the <see cref="OperationResult{T}"/> to indicate a failure due to a not found exception.
    /// </summary>
    /// <typeparam name="T">The type of the result data.</typeparam>
    /// <param name="operationResult">The current <see cref="OperationResult{T}"/> instance.</param>
    /// <param name="message">Optional. A custom message describing the exception. Defaults to "Entity was not found."</param>
    /// <returns>The updated <see cref="OperationResult{T}"/> with the not found exception details.</returns>
    public static OperationResult<T> ReturnWithNotFoundException<T>(this OperationResult<T> operationResult, string? message = null)
    {
        operationResult.Success = false;
        operationResult.InitialException = new IError(HttpStatusCode.NotFound.ToString(), (int)HttpStatusCode.NotFound, message ?? "Entity was not found.");

        return operationResult;
    }

    #endregion Not Found Exception

    #region Bad Request Exception

    /// <summary>
    /// Updates the <see cref="OperationResult"/> to indicate a failure due to a bad request.
    /// </summary>
    /// <param name="operationResult">The current <see cref="OperationResult"/> instance.</param>
    /// <param name="message">Optional. A custom message describing the reason for the bad request. Defaults to "The request was invalid or cannot be processed."</param>
    /// <returns>The updated <see cref="OperationResult"/> with the bad request exception details.</returns>
    public static OperationResult ReturnWithBadRequestException(this OperationResult operationResult, string? message = null)
    {
        operationResult.Success = false;
        operationResult.InitialException = new IError(
            HttpStatusCode.BadRequest.ToString(),
            (int)HttpStatusCode.BadRequest,
            message ?? "The request was invalid or cannot be processed."
        );

        return operationResult;
    }

    /// <summary>
    /// Updates the <see cref="OperationResult{T}"/> to indicate a failure due to a bad request.
    /// </summary>
    /// <typeparam name="T">The type of the result data.</typeparam>
    /// <param name="operationResult">The current <see cref="OperationResult{T}"/> instance.</param>
    /// <param name="message">Optional. A custom message describing the reason for the bad request. Defaults to "The request was invalid or cannot be processed."</param>
    /// <returns>The updated <see cref="OperationResult{T}"/> with the bad request exception details.</returns>
    public static OperationResult<T> ReturnWithBadRequestException<T>(this OperationResult<T> operationResult, string? message = null)
    {
        operationResult.Success = false;
        operationResult.InitialException = new IError(
            HttpStatusCode.BadRequest.ToString(),
            (int)HttpStatusCode.BadRequest,
            message ?? "The request was invalid or cannot be processed."
        );

        return operationResult;
    }

    #endregion Bad Request Exception

    #region Validation Exception

    /// <summary>
    /// Updates the <see cref="OperationResult"/> to indicate a failure due to validation errors in the request.
    /// </summary>
    /// <param name="operationResult">The current <see cref="OperationResult"/> instance.</param>
    /// <param name="errors">A dictionary containing validation errors where the key represents the field name and the value is a list of error messages for that field.</param>
    /// <returns>The updated <see cref="OperationResult"/> with validation exception details and a collection of validation errors.</returns>
    public static OperationResult ReturnWithValidationException(this OperationResult operationResult, Dictionary<string, List<string>> errors)
    {
        operationResult.Success = false;
        operationResult.InitialException = new IError(
            HttpStatusCode.UnprocessableEntity.ToString(),
            (int)HttpStatusCode.UnprocessableEntity,
            "Validation failed for one or more fields."
        );
        operationResult.ValidationErrors = errors;

        return operationResult;
    }

    /// <summary>
    /// Updates the <see cref="OperationResult{T}"/> to indicate a failure due to validation errors in the request.
    /// </summary>
    /// <typeparam name="T">The type of the result data.</typeparam>
    /// <param name="operationResult">The current <see cref="OperationResult{T}"/> instance.</param>
    /// <param name="errors">A dictionary containing validation errors where the key represents the field name and the value is a list of error messages for that field.</param>
    /// <returns>The updated <see cref="OperationResult{T}"/> with validation exception details and a collection of validation errors.</returns>
    public static OperationResult<T> ReturnWithValidationException<T>(this OperationResult<T> operationResult, Dictionary<string, List<string>> errors)
    {
        operationResult.Success = false;
        operationResult.InitialException = new IError(
            HttpStatusCode.UnprocessableEntity.ToString(),
            (int)HttpStatusCode.UnprocessableEntity,
            "Validation failed for one or more fields."
        );
        operationResult.ValidationErrors = errors;

        return operationResult;
    }

    #endregion Validation Exception

    /// <summary>
    /// Creates a new <see cref="OperationResult{U}"/> with a different generic type.
    /// </summary>
    /// <typeparam name="U">The new type for the RelatedObject.</typeparam>
    /// <param name="newRelatedObject">The new value for the RelatedObject.</param>
    /// <returns>A new <see cref="OperationResult{U}"/> with the specified type and value.</returns>
    public static OperationResult<U> ChangeType<U>(this OperationResult operationResult, U? newRelatedObject = default)
    {
        if (newRelatedObject is null)
            return new OperationResult<U>()
            {
                InitialException = operationResult.InitialException,
                Success = operationResult.Success,
            };

        return new OperationResult<U>(newRelatedObject)
        {
            InitialException = operationResult.InitialException,
            Success = operationResult.Success,
        };
    }
}