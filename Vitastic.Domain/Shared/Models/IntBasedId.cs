using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Shared.Models
{
    public abstract class IntBasedId<TId> : StronglyTypedId<TId, int>
        where TId : IntBasedId<TId>
    {
        //Constructor
        protected IntBasedId(int value) : base(value)
        {
        }
        // Override on child class
        protected abstract TId Create(int value);
        //Create id from int
        protected static Result<TId> CreateFrom(
            int value,
            Func<int, TId> factory,
            Error invalidError)
        {
            if (value <= 0)
                return Result.Failure<TId>(invalidError);

            return Result.Success(factory(value));
        }
        //Create id from string
        protected static Result<TId> CreateFrom(
            string value,
            Func<int, TId> factory,
            Error emptyError,
            Error invalidFormatError)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<TId>(emptyError);

            if (!int.TryParse(value, out var intValue))
                return Result.Failure<TId>(invalidFormatError);

            return CreateFrom(intValue, factory, invalidFormatError);
        }
    }
}