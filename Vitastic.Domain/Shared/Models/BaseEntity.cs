using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Shared.Models;

public abstract class BaseEntity<TKey>:IAuditableEntity
    where TKey : IEquatable<TKey>
{
    //----------------------
    // Properties
    //----------------------
    public TKey Id { get; }

    //----------------------
    // Constructors
    //----------------------
    protected BaseEntity() { } // For EF Core

    protected BaseEntity(TKey id) => Id = id;
    //----------------------
    // Audit Properties
    //----------------------
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset  CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public Guid? UpdatedBy { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    //----------------------
    // Equality
    //----------------------
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TKey> other || GetType() != obj.GetType())
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Id.Equals(default(TKey)) || other.Id.Equals(default(TKey)))
            return false;

        return Id.Equals(other.Id);
    }
    //----------------------
    // Behaviors
    //----------------------
    /// <summary>
    /// Set entity creation information
    /// </summary>
    public virtual Result SetCreated(Guid createdBy)
    {
        if (createdBy == Guid.Empty)
            return Error.Validation("Entity.InvalidUser", "شناسه کاربر نامعتبر است");

        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;

        return Result.Success();
    }
    /// <summary>
    /// Set last update information
    /// </summary>
    public Result SetLastModified(Guid modifiedBy)
    {
        if (modifiedBy == Guid.Empty )
            return Error.Validation("Entity.InvalidUser", "شناسه کاربر نامعتبر است");

        UpdatedBy = modifiedBy;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
    /// <summary>
    /// Automatically set update time (without userId)
    /// For Entity internal methods
    /// </summary>
    protected void MarkAsModified()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    //----------------------
    // GetHashCode
    //----------------------
    public override int GetHashCode() =>HashCode.Combine(GetType(),Id);
    //----------------------
    // Operators
    //----------------------
    public static bool operator ==(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }
    public static bool operator !=(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
        => !(left == right);
}
