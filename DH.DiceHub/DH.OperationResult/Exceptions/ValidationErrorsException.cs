﻿namespace DH.OperationResultCore.Exceptions;

public class ValidationErrorsException : Exception
{
    public ValidationErrorsException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationErrorsException(IEnumerable<ValidationError> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public ValidationErrorsException(IDictionary<string, string[]> errors) : this()
    {
        Errors = errors;
    }

    public ValidationErrorsException(string propertyName, string errorMessage) : this()
    {
        Errors = new Dictionary<string, string[]>
    {
        { propertyName, new string[] { errorMessage } }
    };
    }

    public IDictionary<string, string[]> Errors { get; }

    public override string ToString()
    {
        var errorMessages = Errors
            .Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")
            .ToArray();

        return $"{base.ToString()}\nValidation Errors:\n{string.Join("\n", errorMessages)}";
    }

    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationError(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
    }
}