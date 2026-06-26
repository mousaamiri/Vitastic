namespace Vitastic.Domain.Shared.Results;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("یک نتیجه موفق نمی‌تواند حاوی خطا باشد.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("یک نتیجه ناموفق باید حاوی خطا باشد.");
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error Error { get; }
    public static Result Success() => new(true, Error.None);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    public static Result WithErrors(Error[] errors)
    {
        if (errors.Length == 0) return Success();
        // If multiple errors, take the first one or merge it
        return errors[0];
    }
    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;
    internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error) => _value = value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("نمی‌توان مقدار یک نتیجه ناموفق را خواند");
    public new static Result<TValue> WithErrors(Error[] errors)
    {
        if (errors.Length == 0) return Success<TValue>(default!);
        return errors[0];
    }
    public static implicit operator Result<TValue>(TValue? result) => Create(result);
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
}
