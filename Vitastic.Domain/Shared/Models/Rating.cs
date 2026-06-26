using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.Models;

public sealed class Rating : ValueObject
{
    public const decimal MinValue = 1;
    public const decimal MaxValue = 5;
    public const int MaxCommentLength = 1000;

    public decimal Value { get; private set; }
    public string? Comment { get; private set; }

    private Rating(decimal value, string? comment)
    {
        Value = value;
        Comment = comment;
    }

    public static Result<Rating> Create(decimal value, string? comment = null)
    {
        if (value < MinValue || value > MaxValue)
            return RatingErrors.OutOfRange(MinValue, MaxValue);

        if (comment?.Length > MaxCommentLength)
            return RatingErrors.CommentTooLong(MaxCommentLength);

        return Result.Success(new Rating(value, comment));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Comment ?? string.Empty;
    }

    public override string ToString() =>
        string.IsNullOrEmpty(Comment)
            ? Value.ToString("0.0")
            : $"{Value:0.0} - {Comment}";
}

public static class RatingErrors
{
    public static Error OutOfRange(decimal min, decimal max) => Error.Validation(
        "Rating.OutOfRange",
        $"امتیاز باید بین {min} تا {max} باشد");

    public static Error CommentTooLong(int maxLength) => Error.Validation(
        "Rating.CommentTooLong",
        $"نظر نباید بیشتر از {maxLength} کاراکتر باشد");
}
