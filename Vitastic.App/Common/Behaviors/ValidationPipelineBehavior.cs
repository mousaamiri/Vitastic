using FluentValidation;
using MediatR;
using System.Collections.Concurrent;
using System.Reflection;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Behaviors;
/// <summary>
/// Pipeline behavior that validates requests using FluentValidation
/// Returns ValidationResult with errors if validation fails
/// </summary>
public sealed class ValidationPipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    // Cache reflection results per response type
    private static readonly ConcurrentDictionary<Type, Func<Error[], Result>> ValidationResultFactories = [];

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Early return if no validators
        if (!validators.Any())
            return await next(cancellationToken);

        // Run all validators in parallel
        var context = new ValidationContext<TRequest>(request);

        FluentValidation.Results.ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect errors
        Error[] errors = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Select(failure => Error.Validation(
                code: $"{typeof(TRequest).Name}.{failure.PropertyName}",
                message: failure.ErrorMessage
            ))
            .DistinctBy(e => e.Code)
            .ToArray();

        // Return ValidationResult if errors exist
        if (errors.Length > 0)
        {
            return (TResponse)CreateValidationResult(errors);
        }

        return await next(cancellationToken);
    }

    private static Result CreateValidationResult(Error[] errors)
    {
        Func<Error[], Result> factory = ValidationResultFactories.GetOrAdd(
            typeof(TResponse),
            static responseType =>
            {
                if (responseType == typeof(Result))
                {
                    return Result.WithErrors;
                }

                // Generic Result<T>
                Type resultType = responseType.GetGenericArguments()[0];
                Type resultGenericType = typeof(Result<>).MakeGenericType(resultType);
                MethodInfo method = resultGenericType.GetMethod(nameof(Result<object>.WithErrors), BindingFlags.Public | BindingFlags.Static
                )!;

                return errors => (Result)method.Invoke(null, [errors])!;
            });

        return factory(errors);
    }
}
