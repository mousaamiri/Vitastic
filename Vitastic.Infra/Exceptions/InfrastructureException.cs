namespace Vitastic.Infra.Exceptions;

public class InfrastructureException(string errorCode, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public string ErrorCode { get; } = errorCode;
}

