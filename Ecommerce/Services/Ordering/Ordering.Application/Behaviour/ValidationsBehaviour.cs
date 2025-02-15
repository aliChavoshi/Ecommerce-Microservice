using FluentValidation;
using MediatR;
using Ordering.Application.Exceptions;

namespace Ordering.Application.Behaviour;

public class ValidationsBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next();
        // Create a validation context for the request
        var context = new ValidationContext<TRequest>(request);
        //This will run all the validation rules one by one and returns the validation result
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        //now need to check for any failure
        var failures = validationResults
            .SelectMany(e => e.Errors)
            .Where(f => f != null).ToList();
        if (failures.Count != 0) throw new CustomValidationException(failures);

        //On success case 
        return await next();
    }
}