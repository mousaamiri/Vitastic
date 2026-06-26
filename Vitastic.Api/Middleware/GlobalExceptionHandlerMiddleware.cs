using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Vitastic.Domain.Shared.Exceptions;
using Vitastic.Infra.Exceptions;

namespace Vitastic.Api.Middleware
{
    /// <summary>
    /// Middleware for global exception handling
    /// that captures all unhandled exceptions, logs them,
    /// and returns standardized ProblemDetails responses to the client.
    /// </summary>
    public class GlobalExceptionHandlerMiddleware : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IHostEnvironment env)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            //  Structured Logging with Serilog
            _logger.LogError(
                exception,
                "خطای غیرمنتظره رخ داد. نوع: {ExceptionType}, پیام: {Message}, مسیر: {Path}, متد: {Method}",
                exception.GetType().Name,
                exception.Message,
                httpContext.Request.Path,
                httpContext.Request.Method);

            var (statusCode, title, errorCode) = GetErrorDetails(exception);

            var problemDetails = new ProblemDetails
            {
                Type = $"https://httpstatuses.io/{statusCode}", // RFC 7807 STANDARD
                Title = title,
                Status = statusCode,
                Detail = GetErrorMessage(exception),
                Instance = httpContext.Request.Path
            };

            //Add ErrorCode
            problemDetails.Extensions.Add("errorCode", errorCode);

            // Add Timestamp
            problemDetails.Extensions.Add("timestamp", DateTime.UtcNow.ToString("O"));

            //Add TraceId for correlation
            if (httpContext.TraceIdentifier != null)
            {
                problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
            }

            // Full Stack Trace and Inner Exception in Development
            if (_env.IsDevelopment())
            {
                problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
                problemDetails.Extensions.Add("innerException", exception.InnerException?.Message);
            }

            // Exception-handling specific for FluentValidation
            if (exception is FluentValidation.ValidationException validationException)
            {
                var validationErrors = validationException.Errors
                    .GroupBy(e => e.PropertyName, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        g => char.ToLowerInvariant(g.Key[0]) + g.Key[1..], // camelCase
                        g => g.Select(e => e.ErrorMessage).ToArray(),
                        StringComparer.Ordinal);

                problemDetails.Extensions.Add("errors", validationErrors);

                // Logging validation errors separately for better visibility
                _logger.LogWarning(
                    "خطاهای اعتبارسنجی: {ValidationErrors}",
                    string.Join(", ", validationErrors.SelectMany(x => x.Value)));
            }

            // Logging critical errors (500) with more severity
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogCritical(
                    exception,
                    "خطای حیاتی در سرور! Exception: {ExceptionType}",
                    exception.GetType().FullName);
            }

            var context = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json; charset=utf-8";

            return await _problemDetailsService.TryWriteAsync(context);
        }

        /// <summary>
        /// Details of error based on exception type,
        /// mapped to appropriate HTTP status codes and messages.
        /// </summary>
        private static (int StatusCode, string Title, string ErrorCode) GetErrorDetails(Exception exception)
        {
            return exception switch
            {
                // ── Domain Exceptions ──────────────────────────
                BusinessRuleViolationException ex => (StatusCodes.Status400BadRequest, "نقض قانون کسب‌وکار", ex.ErrorCode),
                DuplicateEntityException => (StatusCodes.Status409Conflict, "خطای تکراری", "Entity.Duplicate"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "دسترسی ممنوع", "Auth.Forbidden"),
                EntityNotFoundException => (StatusCodes.Status404NotFound, "یافت نشد", "Resource.NotFound"),
                UniqueConstraintViolatedException => (StatusCodes.Status409Conflict, "مقدار تکراری", "Database.UniqueConstraintViolated"),
                Domain.Shared.Exceptions.ValidationException => (StatusCodes.Status422UnprocessableEntity, "خطای اعتبارسنجی دامنه",
                        "Validation.DomainError"),

                // ── FluentValidation Exceptions ───────────────────
                FluentValidation.ValidationException => (StatusCodes.Status422UnprocessableEntity, "خطای اعتبارسنجی",
                    "FluentValidation.Error"),
                // ── System Exceptions ───────────────────
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "عدم احراز هویت", "Auth.Unauthorized"),
                ArgumentException or ArgumentNullException => (StatusCodes.Status400BadRequest, "پارامتر نامعتبر", "Argument.Invalid"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "یافت نشد", "Resource.NotFound"),
                TimeoutException => (StatusCodes.Status408RequestTimeout, "زمان درخواست به پایان رسید", "Request.Timeout"),
                NotImplementedException => (StatusCodes.Status501NotImplemented, "پیاده‌سازی نشده", "Feature.NotImplemented"),

                // ── Infrastructure Exceptions ──────────────────
                DbUpdateException or DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "تداخل در بروزرسانی", "Database.Conflict"),
                ConcurrencyConflictException => (StatusCodes.Status409Conflict, "تداخل همزمانی", "Database.ConcurrencyConflict"),
                FileStorageException => (StatusCodes.Status500InternalServerError, "خطای ذخیره‌سازی فایل", "FileStorage.Error"),
                RepositoryException => (StatusCodes.Status500InternalServerError, "خطای مخزن داده", "Repository.Error"),
                InfrastructureException => (StatusCodes.Status500InternalServerError, "خطای زیرساخت", "Infrastructure.Error"),

                // ── External Library Exceptions ────────────────
                InvalidOperationException => (StatusCodes.Status400BadRequest, "عملیات نامعتبر", "Operation.Invalid"),
                JsonSerializationException => (StatusCodes.Status400BadRequest, "خطای تجزیه JSON", "Json.SerializationError"),
                // Default Error
                _ => (StatusCodes.Status500InternalServerError, "خطای سرور", "Server.InternalError")
            };
        }

        /// <summary>
        /// Get user-friendly error message based on exception type.
        /// </summary>
        private string GetErrorMessage(Exception exception)
        {
            // Production-friendly messages without exposing sensitive details
            if (!_env.IsDevelopment())
            {
                return exception switch
                {
                    FluentValidation.ValidationException => "اطلاعات ارسالی معتبر نیست. لطفاً ورودی‌های خود را بررسی کنید",
                    UnauthorizedAccessException => "شما به این منبع دسترسی ندارید",
                    KeyNotFoundException => "اطلاعات درخواستی یافت نشد",
                    TimeoutException => "زمان درخواست به پایان رسید. لطفاً دوباره تلاش کنید",
                    InvalidOperationException => exception.Message,
                    _ => "خطایی در پردازش درخواست شما رخ داد. لطفاً با پشتیبانی تماس بگیرید"
                };
            }

            // Full details in development for easier debugging
            return exception.ToString();
        }
    }
}
