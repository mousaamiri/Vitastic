using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Shared.Models
{
    public abstract class GuidBasedId<TId>:StronglyTypedId<TId,Guid>
        where TId : GuidBasedId<TId>
    {
        //Constructor
        protected GuidBasedId(Guid value) : base(value)
        {
        }

        //Factory methods overloads by child classes
        protected abstract TId Create(Guid value);
        //Generate a new
        public static TId New<T>() where T :GuidBasedId<TId>,new()
        {
            // Don't use reflection. child must implement this;;.
            throw new NotSupportedException($" از {typeof(TId).Name}  مستقیما در کلاس فرزند استفاده کنید.");
        }
        //Generate a new id from guid
        protected static Result<TId> CreateFrom(Guid value, Func<Guid,Result<TId>> factory, Error emptyError)
        {
            if (value == Guid.Empty)
                return emptyError;
            return factory(value);
        }
        //Generate a new id from string
        protected static Result<TId> CreateFrom(string value, Func<Guid, Result<TId>> factory,
            Error emptyError,
            Error invalidFormatError)
        {
            if(string.IsNullOrWhiteSpace(value))
                return emptyError;
            if (!Guid.TryParse(value, out Guid guid))
                return invalidFormatError;
            return CreateFrom(guid, factory,emptyError);
        }
    }
}