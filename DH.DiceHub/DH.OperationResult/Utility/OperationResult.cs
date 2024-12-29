using System.Text.Json.Serialization;

namespace DH.OperationResultCore.Utility;

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
        get => _validationErrors;
        set => _validationErrors = value;
    }

    [JsonPropertyName("success")]
    public bool Success
    {
        get => _success;
        set => _success = value;
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
            _initialException = value;
            _success = false;
        }
    }

    /////// <summary>
    /////// Gets an <see cref="ILoggerService"/> that can be used to log errors internally.
    /////// </summary>
    //protected ILoggerService Logger { get; }

    public void AppendValidationError(string errorMessage, string propertyName)
    {
        if (_initialException == null)
        {
            _initialException = new IError("Validation", 422, "Validation Message");
        }

        //this._success = false;
        if (!_validationErrors.Any(x => x.Key == propertyName))
            _validationErrors.Add(propertyName, new List<string>());

        _validationErrors.First(x => x.Key == propertyName).Value.Add(errorMessage);
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
        RelatedObject = resultObject;
    }

    /// <summary>
    /// Gets or sets the related object of the operation.
    /// </summary>
    public T? RelatedObject { get; set; }
}