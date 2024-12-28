using DH.Statistics.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace DH.Statistics.Domain.Models;

/// <summary>
/// A Class for a system operation result, with a generic TKey.
/// </summary>
[Serializable]
public class OperationResult
{
    private Dictionary<string, List<string>> _validationErrors = new Dictionary<string, List<string>>();
    private bool _success = true;
    private IError? _initialException { get; set; }
    [JsonPropertyName("validationErrors")]
    public Dictionary<string, List<string>> ValidationErrors
    {
        get => this._validationErrors;
        set => this._validationErrors = value;
    }

    [JsonPropertyName("success")]
    public bool Success
    {
        get => this._success;
        set => this._success = value;
    }

    /// <summary>
    /// Gets or sets the first exception that resulted from the operation.
    /// </summary>
    [JsonPropertyName("initialException")]
    public IError? InitialException
    {
        get => _initialException;
        set
        {
            this._initialException = value;
            this._success = false;
        }
    }

    /////// <summary>
    /////// Gets an <see cref="ILoggerService"/> that can be used to log errors internally.
    /////// </summary>
    //protected ILoggerService Logger { get; }

    public void AppendValidationError(string errorMessage, string propertyName)
    {
        if (this._initialException == null)
        {
            this._initialException = new IError(ErrorCode.Validation.ToString(),(int)ErrorCode.Validation,"Validation Message");
        }

        //this._success = false;
        if (!this._validationErrors.Any(x => x.Key == propertyName))
            this._validationErrors.Add(propertyName, new List<string>());

        this._validationErrors.First(x => x.Key == propertyName).Value.Add(errorMessage);
    }
}

/// <summary>
/// Represents a generic system operation.
/// </summary>
public class OperationResult<T> : OperationResult
{
    public OperationResult()
    {
    }

    public OperationResult(T resultObject)
    {
        this.RelatedObject = resultObject;
    }

    /// <summary>
    /// Gets or sets the related object of the operation.
    /// </summary>
    public T? RelatedObject { get; set; }
}