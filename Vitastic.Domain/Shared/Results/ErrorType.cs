namespace Vitastic.Domain.Shared.Results;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    Failure,
    Unauthorized,
    Forbidden,
    Verification
}
