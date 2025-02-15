using FluentValidation.Results;

namespace Ordering.Application.Exceptions;

public class CustomValidationException() : ApplicationException("One or more validation failures have occurred.")
{
    public Dictionary<string, string[]> Errors { get; set; } = new();

    public CustomValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage + " " + e.ErrorCode)
            .ToDictionary(failureGroup => failureGroup.Key,
                failureGroup => failureGroup.ToArray());
    }
}