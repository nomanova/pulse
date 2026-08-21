using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using FluentValidation;
using Pulse.App.Common.Dispatcher;

namespace Pulse.App.Common.Validation;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var validator in validators)
        {
            var validationResult = await validator.ValidateAsync(context, cancellationToken);
            if (!validationResult.IsValid)
            {
                failures.AddRange(validationResult.Errors);
            }
        }

        if (failures.Count > 0)
        {
            var errors = failures.ConvertAll(failure =>
                Error.Validation(failure.PropertyName, failure.ErrorMessage));
            return (dynamic)errors;
        }

        return await next();
    }
}