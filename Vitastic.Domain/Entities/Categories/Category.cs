using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Categories;

public sealed class Category : AggregateRoot<CategoryId>
{
    // ------------------------
    // Properties
    // ------------------------
    public CategoryName Name { get; private set; }
    public Slug Slug { get; private set; }
    public Description? Description { get; private set; }
    public CategoryId? ParentCategoryId { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    // ------------------------
    // Constructors
    // ------------------------
    private Category(CategoryId id,
        CategoryName name,
        Slug slug,
        int? displayOrder, CategoryId? parentCategoryId) : base(id)
    {
        Name = name;
        Slug = slug;
        DisplayOrder = displayOrder ?? 1;
        IsActive = true;
        ParentCategoryId = parentCategoryId;
    }

    private Category() : base()
    {
    } // EF Core

    // ------------------------
    // Factory Method
    // ------------------------
    public static Result<Category> Create(string name, string slug, int? displayOrder,Guid? parentCategoryId=null)
    {
        var nameResult = CategoryName.Create(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var slugResult = Slug.Create(slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        if (displayOrder.HasValue)
        {
            if (displayOrder < 1)
                return CategoryErrors.InvalidDisplayOrder;
        }

        CategoryId? parentId=null;
        if (parentCategoryId is not null)
        {
            Result<CategoryId> result = CategoryId.CreateFrom(parentCategoryId.Value);
            if (result.IsFailure)
                return result.Error;
            parentId = result.Value;
        }
        return new Category(
            CategoryId.New(),
            nameResult.Value,
            slugResult.Value,
            displayOrder,
            parentId
        );
    }

    // ------------------------
    // Behaviors
    // ------------------------
    public Result UpdateName(string newName)
    {
        Result<CategoryName> nameResult = CategoryName.Create(newName);
        if (nameResult.IsFailure)
            return nameResult.Error;

        Name = nameResult.Value;

        return Result.Success();
    }

    public Result UpdateSlug(string newSlug)
    {
        Result<Slug> slugResult = Slug.Create(newSlug);
        if (slugResult.IsFailure) return slugResult.Error;
        Slug = slugResult.Value;
        return Result.Success();
    }

    public Result SetDescription(string description)
    {
        var dscCreateResult = Description.Create(description);
        if (dscCreateResult.IsFailure)
            return dscCreateResult.Error;
        Description = dscCreateResult.Value;

        return Result.Success();
    }

    public Result SetParentCategory(CategoryId? parentCategoryId)
    {
        if (parentCategoryId != null && parentCategoryId.Equals(Id))
            return Result.Success();
            // return CategoryErrors.CannotBeOwnParent;

        ParentCategoryId = parentCategoryId;

        return Result.Success();
    }

    public Result RemoveParentCategory()
    {
        ParentCategoryId = null;
        return Result.Success();
    }

    public Result UpdateDisplayOrder(int order)
    {
        if (order < 1)
            return CategoryErrors.InvalidDisplayOrder;

        DisplayOrder = order;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
            return CategoryErrors.AlreadyActive;

        IsActive = true;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return CategoryErrors.AlreadyInactive;

        IsActive = false;
        return Result.Success();
    }

    public bool IsSubCategory() => ParentCategoryId is not null;
}
