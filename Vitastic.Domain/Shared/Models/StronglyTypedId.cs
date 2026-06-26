using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Shared.Models;
public abstract class StronglyTypedId<TId,TValue>: ValueObject<TValue>,IEquatable<TId>
    where TId : StronglyTypedId<TId,TValue>
    where TValue : notnull
{
    //Constructor
    protected StronglyTypedId(TValue value):base(value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    //Overrider
    protected override IEnumerable<object> GetEqualityComponents()
       {
           yield return Value;
       }
    public override string ToString() => Value.ToString() ?? string.Empty;

    // Implicit conversion to T
    public static implicit operator TValue(StronglyTypedId<TId,TValue> id) => id.Value;
    protected bool Equals(StronglyTypedId<TId, TValue> other)
        => base.Equals(other) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);

    public bool Equals(TId? other)
    {
        if(other is null)
            return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((StronglyTypedId<TId, TValue>)obj);
    }
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Value);
}
