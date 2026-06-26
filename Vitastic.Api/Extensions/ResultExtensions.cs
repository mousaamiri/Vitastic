using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Features.Base;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Extensions;

internal static class ResultExtensions
{
    public static ApiResponse ToApiResponse(this Result result, string? message = null)
    {
        if (result.IsSuccess)
        {
            return ApiResponse.Success(message);
        }

        return ApiResponse.Fail(
            message: message ?? result.Error.Message,
            errorType: result.Error.ErrorType,
            errors: [result.Error.ToString()]
        );
    }

    // ⭐⭐⭐ THE IMPORTANT ONE ⭐⭐⭐
    // For Result<TSource> → ApiResponse<TDest> (WITH MAPPING)
    public static ApiResponse<TDest> ToApiResponse<TSource, TDest>(
        this Result<TSource> result,
        Func<TSource, TDest> mapper,
        string? failMessage = null)
    {
        if (result.IsSuccess)
        {
            TDest mapped = mapper(result.Value);   // map DTO → Response
            return ApiResponse<TDest>.Success(mapped);
        }

        return ApiResponse<TDest>.Fail(
            message: failMessage ?? result.Error.Message,
            errorType: result.Error.ErrorType,
            errors: [result.Error.ToString()]);
    }
    public static ApiResponse<TDest> ToApiResponse<TDest>(
        this Result result,
        string? message = null)
    {
        return ApiResponse<TDest>.Fail(
            message: message ?? result.Error.Message,
            errorType: result.Error.ErrorType,
            errors: [result.Error.ToString()]
        );
    }

}
