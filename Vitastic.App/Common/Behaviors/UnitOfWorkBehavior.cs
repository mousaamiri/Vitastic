using System.Reflection;
using System.Transactions;
using EntityFramework.Exceptions.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vitastic.App.Data;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Behaviors;

/// <summary>
/// Pipeline behavior that manages unit of work and transactions for commands
/// Returns failure result if database constraints are violated
/// </summary>
public sealed class UnitOfWorkBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only process commands, skip queries
        if (IsNotCommand() && IsNotEvent())
            return await next(cancellationToken);

        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        TResponse response = await next(cancellationToken);

        // Rollback if handler returned failure
        if (response is Result { IsFailure: true })
            return response;

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            transactionScope.Complete();
        }
        catch (UniqueConstraintException ex)
        {
            logger.LogError(ex, "Unique constraint violation occurred");
            return CreateFailureResult("این رکورد قبلاً وجود دارد");
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update error occurred");
            return CreateFailureResult("خطا در ذخیره‌سازی داده‌ها. لطفاً دوباره تلاش کنید");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during transaction");
            return CreateFailureResult("خطای نامشخصی رخ داد");
        }

        return response;
    }

    /// <summary>
    /// Checks if request is a command (class name ends with "Command")
    /// </summary>
    private static bool IsNotCommand()
        => !typeof(TRequest).Name.EndsWith("Command");
    /// <summary>
    /// Checks if request is a event (class name ends with "event")
    /// </summary>
    private static bool IsNotEvent()
        => !typeof(TRequest).Name.EndsWith("Event");
    /// <summary>
    /// Creates a failure result with Persian error message
    /// </summary>
    private static TResponse CreateFailureResult(string persianMessage)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(Error.Failure("Database", persianMessage));

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];
            Type resultType = typeof(Result<>).MakeGenericType(valueType);

            MethodInfo? failureMethod = resultType.GetMethod(
                nameof(Result.Failure),
                BindingFlags.Public | BindingFlags.Static,
                null,
                [typeof(Error)],
                null
            );

            return (TResponse)failureMethod!.Invoke(null, [Error.Failure("Database", persianMessage)])!;
        }

        throw new InvalidOperationException($"Response type '{typeof(TResponse).Name}' must derive from Result");
    }
}
