// Domain/Tags/Tag.cs

using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Tags;

public sealed class Tag : AggregateRoot<TagId>
{
    // ------------------------
    // Properties
    // ------------------------
    public TagName Name { get; private set; }
    public int UsageCount { get; private set; }
    public bool IsActive { get; private set; }

    // ------------------------
    // Constructors
    // ------------------------
    private Tag(TagId id, TagName name) : base(id)
    {
        Name = name;
        UsageCount = 0;
        IsActive = true;
    }

    private Tag() : base() { } // EF Core

    // ------------------------
    // Factory Method
    // ------------------------
    public static Result<Tag> Create(string name)
    {
        var nameResult = TagName.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        return new Tag(TagId.New(), nameResult.Value);
    }

    // ------------------------
    // Behaviors
    // ------------------------
    public Result UpdateName(string newName)
    {
        Result<TagName> tagNameResult = TagName.Create(newName);
        if(tagNameResult.IsFailure)
            return tagNameResult.Error;


        Name = tagNameResult.Value;
        return Result.Success();
    }

    public void IncrementUsage()
    {
        UsageCount++;
    }

    public void DecrementUsage()
    {
        if (UsageCount > 0)
        {
            UsageCount--;
        }
    }

    public Result Activate()
    {
        if (IsActive)
            return TagErrors.AlreadyActive;

        IsActive = true;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return TagErrors.AlreadyInactive;

        IsActive = false;

        return Result.Success();
    }
}
