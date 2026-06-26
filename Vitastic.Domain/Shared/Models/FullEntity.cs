using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Shared.Models;

public abstract class FullEntity<TKey> : BaseEntity<TKey>,
    ISoftDeleteEntity
    where TKey : IEquatable<TKey>

{
    //----------------------
    // Constructors
    //----------------------
    protected FullEntity() { } // For Ef
    protected FullEntity(TKey id):base(id) { }


    //----------------------
    // SoftDelete Properties
    //----------------------
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    //-----------------------
    // Behaviors
    //-----------------------

    /// <summary>
    /// Soft delete entity
    /// </summary>
    public Result MarkDeleted(Guid deletedBy)
    {
        if (deletedBy == Guid.Empty)
            return Error.Validation("Entity.InvalidUser", "شناسه کاربر نامعتبر است");

        if (IsDeleted)
            return Error.Conflict("Entity.AlreadyDeleted", "این موجودیت قبلاً حذف شده است");

        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
    /// <summary>
    /// Recover deleted entity
    /// </summary>
    public Result Restore()
    {
        if (!IsDeleted)
            return Error.Conflict("Entity.NotDeleted", "این موجودیت حذف نشده است");

        IsDeleted = false;
        DeletedBy = default;
        DeletedAt = null;

        return Result.Success();
    }
}
