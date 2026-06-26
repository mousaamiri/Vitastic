using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Behaviors;

/// <summary>
/// Pipeline behavior that logs all requests/responses with execution time
/// </summary>
public class LoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation(
                "Executing request: {RequestName} at {Timestamp}",
                requestName,
                DateTime.UtcNow);

            TResponse result = await next(cancellationToken);
            stopwatch.Stop();

            if (result.IsFailure)
            {
                logger.LogError(
                    "Request failed: {RequestName} in {ElapsedMilliseconds}ms. Error Code: {ErrorCode}, Message: {ErrorMessage}",
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    result.Error.Code,
                    result.Error.Message);
            }
            else
            {
                logger.LogInformation(
                    "Request completed successfully: {RequestName} in {ElapsedMilliseconds}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(
                ex,
                "Request threw exception: {RequestName} in {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
