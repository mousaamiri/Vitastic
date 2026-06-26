using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Discounts.Enums;
using Vitastic.Domain.Entities.Discounts.Events;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Discounts;

public sealed class Discount : AggregateRoot<DiscountId>
{
    private readonly HashSet<UserId> _usedByUserIds = [];
    private readonly HashSet<CourseId> _applicableCourseIds = [];
    private readonly HashSet<CategoryId> _applicableCategoryIds = [];
    private readonly HashSet<InstructorId> _applicableInstructorIds = [];

    // ------------------------
    // Properties
    // ------------------------
    public DiscountCode Code { get; private set; }
    public Title Title { get; private set; }
    public Description? Description { get; private set; }

    public DiscountType Type { get; private set; }
    public DiscountScope Scope { get; private set; }

    // For percentage discounts: store the percentage value (e.g., 20 for 20%)
    public decimal PercentageValue { get; private set; } = 0;

    // For fixed amount discounts: store the money value
    public Money? FixedAmountValue { get; private set; }

    public Money? MinimumOrderAmount { get; private set; }
    public Money? MaximumDiscountAmount { get; private set; }

    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }

    public int? UsageLimit { get; private set; }
    public int UsedCount => _usedByUserIds.Count;

    public bool IsActive { get; private set; }
    public bool IsSingleUse { get; private set; }

    // Read-only collections
    public IReadOnlyCollection<UserId> UsedByUserIds => _usedByUserIds;
    public IReadOnlyCollection<CourseId> ApplicableCourseIds => _applicableCourseIds;
    public IReadOnlyCollection<CategoryId> ApplicableCategoryIds => _applicableCategoryIds;
    public IReadOnlyCollection<InstructorId> ApplicableInstructorIds => _applicableInstructorIds;

    // ------------------------
    // Constructors
    // ------------------------
    private Discount(
        DiscountId id,
        DiscountCode code,
        Title title,
        DiscountType type,
        DiscountScope scope,
        DateTimeOffset startDate,
        DateTimeOffset endDate) : base(id)
    {
        Code = code;
        Title = title;
        Type = type;
        Scope = scope;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
        IsSingleUse = false;
    }

    private Discount() : base()
    {
    } // EF Core

    // ------------------------
    // Factory Methods
    // ------------------------

    /// <summary>
    /// Creates a percentage-based discount
    /// </summary>
    public static Result<Discount> CreatePercentage(
        DiscountCode code,
        Title title,
        DiscountScope scope,
        decimal percentage,
        DateTimeOffset startDate,
        DateTimeOffset endDate)
    {
        if (percentage <= 0 || percentage > 100)
            return DiscountErrors.InvalidPercentage;

        if (startDate >= endDate)
            return DiscountErrors.InvalidDateRange;

        if (endDate < DateTime.UtcNow)
            return DiscountErrors.EndDateInPast;

        var discount = new Discount(
            DiscountId.New(),
            code,
            title,
            DiscountType.Percentage,
            scope,
            startDate,
            endDate
        )
        {
            PercentageValue = percentage,
            FixedAmountValue = null
        };

        discount.RaiseDomainEvent(DiscountCreatedDomainEvent.Create(
            discount.Id,
            discount.Code,
            discount.Type
        ));

        return discount;
    }

    /// <summary>
    /// Creates a fixed amount discount
    /// </summary>
    public static Result<Discount> CreateFixedAmount(
        DiscountCode code,
        Title title,
        DiscountScope scope,
        Money amount,
        DateTimeOffset startDate,
        DateTimeOffset endDate)
    {
        if (startDate >= endDate)
            return DiscountErrors.InvalidDateRange;

        if (endDate < DateTime.UtcNow)
            return DiscountErrors.EndDateInPast;

        var discount = new Discount(
            DiscountId.New(),
            code,
            title,
            DiscountType.FixedAmount,
            scope,
            startDate,
            endDate
        );

        discount.PercentageValue = 0;
        discount.FixedAmountValue = amount;

        discount.RaiseDomainEvent(DiscountCreatedDomainEvent.Create(
            discount.Id,
            discount.Code,
            discount.Type
        ));

        return discount;
    }

    // ------------------------
    // Configuration
    // ------------------------

    public Result SetDescription(Description description)
    {
        if (description is null)
            return Result.Success();

        Description = description;
        return Result.Success();
    }

    public Result SetMinimumOrderAmount(Money minimumAmount)
    {
        if (minimumAmount is null)
            return DiscountErrors.InvalidMinimumAmount;

        if (minimumAmount.Value <= 0)
            return DiscountErrors.MinimumAmountMustBePositive;
        if (MaximumDiscountAmount != null && MaximumDiscountAmount.HasValue &&
            MaximumDiscountAmount.Value <= minimumAmount.Value)
            return DiscountErrors.MinMustBeBiggerThanMax;
        MinimumOrderAmount = minimumAmount;
        return Result.Success();
    }

    public Result SetMaximumDiscountAmount(Money maximumAmount)
    {
        if (Type != DiscountType.Percentage)
            return DiscountErrors.MaxDiscountOnlyForPercentage;

        if (maximumAmount is null)
            return DiscountErrors.InvalidMaximumAmount;

        if (maximumAmount.Value <= 0)
            return DiscountErrors.MaximumAmountMustBePositive;
        if (MinimumOrderAmount != null && MinimumOrderAmount.HasValue &&
            MinimumOrderAmount.Value >= maximumAmount.Value)
            return DiscountErrors.MaxMustBeBiggerThanMin;

        MaximumDiscountAmount = maximumAmount;
        return Result.Success();
    }

    public Result SetUsageLimit(int limit)
    {
        if (limit <= 0)
            return DiscountErrors.UsageLimitMustBePositive;

        if (UsedCount >= limit)
            return DiscountErrors.UsageLimitExceeded;

        UsageLimit = limit;
        return Result.Success();
    }

    public Result MakeSingleUse()
    {
        IsSingleUse = true;
        UsageLimit = null;
        return Result.Success();
    }

    // ------------------------
    // Scope Configuration - Courses
    // ------------------------

    /// <summary>
    /// Adds a course to the applicable courses list
    /// </summary>
    public Result AddCourse(CourseId courseId)
    {
        if (Scope != DiscountScope.SpecificCourses)
            return DiscountErrors.ScopeMismatch;

        if (courseId is null)
            return DiscountErrors.InvalidCourseId;

        if (!_applicableCourseIds.Add(courseId))
            return DiscountErrors.CourseAlreadyAdded;

        return Result.Success();
    }

    /// <summary>
    /// Removes a course from the applicable courses list
    /// </summary>
    public Result RemoveCourse(CourseId courseId)
    {
        if (!_applicableCourseIds.Remove(courseId))
            return DiscountErrors.CourseNotFound;

        return Result.Success();
    }

    /// <summary>
    /// Clears all courses from the applicable list
    /// </summary>
    public Result ClearCourses()
    {
        _applicableCourseIds.Clear();
        return Result.Success();
    }

    // ------------------------
    // Scope Configuration - Categories
    // ------------------------

    /// <summary>
    /// Adds a category to the applicable categories list
    /// </summary>
    public Result AddCategory(CategoryId categoryId)
    {
        if (Scope != DiscountScope.SpecificCategories)
            return DiscountErrors.ScopeMismatch;

        if (categoryId is null)
            return DiscountErrors.InvalidCategoryId;

        if (!_applicableCategoryIds.Add(categoryId))
            return DiscountErrors.CategoryAlreadyAdded;

        return Result.Success();
    }

    /// <summary>
    /// Removes a category from the applicable categories list
    /// </summary>
    public Result RemoveCategory(CategoryId categoryId)
    {
        if (!_applicableCategoryIds.Remove(categoryId))
            return DiscountErrors.CategoryNotFound;

        return Result.Success();
    }

    /// <summary>
    /// Clears all categories from the applicable list
    /// </summary>
    public Result ClearCategories()
    {
        _applicableCategoryIds.Clear();
        return Result.Success();
    }

    // ------------------------
    // Scope Configuration - Instructors
    // ------------------------

    /// <summary>
    /// Adds an instructor to the applicable instructors list
    /// </summary>
    public Result AddInstructor(InstructorId instructorId)
    {
        if (Scope != DiscountScope.SpecificInstructors)
            return DiscountErrors.ScopeMismatch;

        if (instructorId is null)
            return DiscountErrors.InvalidInstructorId;

        if (!_applicableInstructorIds.Add(instructorId))
            return DiscountErrors.InstructorAlreadyAdded;

        return Result.Success();
    }

    /// <summary>
    /// Removes an instructor from the applicable instructors list
    /// </summary>
    public Result RemoveInstructor(InstructorId instructorId)
    {
        if (!_applicableInstructorIds.Remove(instructorId))
            return DiscountErrors.InstructorNotFound;

        return Result.Success();
    }

    /// <summary>
    /// Clears all instructors from the applicable list
    /// </summary>
    public Result ClearInstructors()
    {
        _applicableInstructorIds.Clear();
        return Result.Success();
    }

    // ------------------------
    // Validation
    // ------------------------

    /// <summary>
    /// Checks if the discount is currently valid
    /// </summary>
    public bool IsValid()
    {
        if (!IsActive)
            return false;

        var now = DateTime.UtcNow;
        if (now < StartDate || now > EndDate)
            return false;

        if (UsageLimit.HasValue && UsedCount >= UsageLimit.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the discount can be applied by a specific user
    /// </summary>
    public bool CanBeAppliedByUser(UserId userId)
    {
        if (!IsValid())
            return false;

        if (IsSingleUse && _usedByUserIds.Contains(userId))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the discount is applicable to a specific course
    /// </summary>
    public bool IsApplicableToCourse(CourseId courseId)
    {
        return Scope switch
        {
            DiscountScope.Global => true,
            DiscountScope.SpecificCourses => _applicableCourseIds.Contains(courseId),
            _ => false // Categories and Instructors need to be checked via Domain Service
        };
    }

    /// <summary>
    /// Calculates the discount amount for a given order total
    /// </summary>
    public Result<Money> CalculateDiscountAmount(Money orderTotal)
    {
        if (!IsValid())
            return DiscountErrors.DiscountNotValid;

        // Check minimum order amount requirement
        if (MinimumOrderAmount is not null)
        {
            if (orderTotal.Currency != MinimumOrderAmount.Currency)
                return DiscountErrors.CurrencyMismatch;

            if (orderTotal.Value < MinimumOrderAmount.Value)
                return DiscountErrors.OrderBelowMinimum(MinimumOrderAmount.Value);
        }

        Money discountAmount;

        if (Type == DiscountType.Percentage)
        {
            // Calculate percentage discount
            var percentage = PercentageValue / 100m;
            var calculatedAmount = orderTotal.Value * percentage;

            // Apply maximum discount cap if set
            if (MaximumDiscountAmount is not null &&
                calculatedAmount > MaximumDiscountAmount.Value)
            {
                calculatedAmount = MaximumDiscountAmount.Value;
            }

            var amountResult = Money.Create(calculatedAmount, orderTotal.Currency.Code);
            if (amountResult.IsFailure)
                return amountResult.Error;

            discountAmount = amountResult.Value;
        }
        else // FixedAmount
        {
            if (FixedAmountValue!.Currency != orderTotal.Currency)
                return DiscountErrors.CurrencyMismatch;

            // Discount cannot exceed order total
            discountAmount = FixedAmountValue.Value > orderTotal.Value
                ? orderTotal
                : FixedAmountValue;
        }

        return discountAmount;
    }

    // ------------------------
    // Application
    // ------------------------

    /// <summary>
    /// Applies the discount for a specific user
    /// </summary>
    public Result Apply(UserId userId)
    {
        if (!CanBeAppliedByUser(userId))
            return DiscountErrors.CannotBeApplied;

        if (IsSingleUse && _usedByUserIds.Contains(userId))
            return DiscountErrors.AlreadyUsedByUser;

        if (UsageLimit.HasValue && UsedCount >= UsageLimit.Value)
            return DiscountErrors.UsageLimitReached;

        _usedByUserIds.Add(userId);

        RaiseDomainEvent(DiscountUsedDomainEvent.Create(
            Id,
            userId,
            UsedCount
        ));
        return Result.Success();
    }

    // ------------------------
    // Status Management
    // ------------------------

    /// <summary>
    /// Activates the discount
    /// </summary>
    public Result Activate()
    {
        if (IsActive)
            return DiscountErrors.AlreadyActive;

        if (EndDate < DateTime.UtcNow)
            return DiscountErrors.CannotActivateExpired;

        IsActive = true;

        RaiseDomainEvent(DiscountActivatedDomainEvent.Create(Id));

        return Result.Success();
    }

    /// <summary>
    /// Deactivates the discount
    /// </summary>
    public Result Deactivate()
    {
        if (!IsActive)
            return DiscountErrors.AlreadyInactive;

        IsActive = false;

        RaiseDomainEvent(DiscountDeactivatedDomainEvent.Create(Id));

        return Result.Success();
    }

    /// <summary>
    /// Extends the discount end date
    /// </summary>
    public Result ExtendEndDate(DateTimeOffset newEndDate)
    {
        if (newEndDate <= EndDate)
            return DiscountErrors.NewEndDateMustBeLater;

        if (newEndDate < DateTime.UtcNow)
            return DiscountErrors.EndDateInPast;

        var oldEndDate = EndDate;
        EndDate = newEndDate;

        RaiseDomainEvent(DiscountExtendedDomainEvent.Create(
            Id,
            oldEndDate,
            newEndDate
        ));

        return Result.Success();
    }

    // ------------------------
    // Query Methods
    // ------------------------

    public bool IsExpired() => DateTime.UtcNow > EndDate;

    public bool HasStarted() => DateTime.UtcNow >= StartDate;

    public bool IsUsageLimitReached() =>
        UsageLimit.HasValue && UsedCount >= UsageLimit.Value;

    public int GetRemainingUsage() =>
        UsageLimit.HasValue ? Math.Max(0, UsageLimit.Value - UsedCount) : int.MaxValue;

    public TimeSpan? GetTimeUntilExpiry()
    {
        if (IsExpired())
            return null;

        return EndDate - DateTime.UtcNow;
    }

    public bool HasAnyCourses() => _applicableCourseIds.Any();

    public bool HasAnyCategories() => _applicableCategoryIds.Any();

    public bool HasAnyInstructors() => _applicableInstructorIds.Any();

    public decimal GetDiscountPercentage() =>
        Type == DiscountType.Percentage ? PercentageValue : 0;

    /// <summary>
    /// Updates the discount properties
    /// </summary>
    public Result Update(
        DiscountCode code,
        Title title,
        DateTime startDate,
        DateTime endDate,
        Money? minimumOrderAmount,
        Money? maximumDiscountAmount,
        int? maxUsageCount)
    {
        if (startDate >= endDate)
            return DiscountErrors.InvalidDateRange;

        if (endDate < DateTime.UtcNow)
            return DiscountErrors.EndDateInPast;

        if (UsedCount > 0)
        {
            if (endDate > EndDate)
                EndDate = endDate;

            if (minimumOrderAmount != null)
            {
                var minResult = SetMinimumOrderAmount(minimumOrderAmount);
                if (minResult.IsFailure) return minResult.Error;
            }
            else
            {
                MinimumOrderAmount = null;
            }

            if (maximumDiscountAmount != null && Type == DiscountType.Percentage)
            {
                var maxResult = SetMaximumDiscountAmount(maximumDiscountAmount);
                if (maxResult.IsFailure) return maxResult.Error;
            }
            else if (Type == DiscountType.Percentage)
            {
                MaximumDiscountAmount = null;
            }

            if (maxUsageCount.HasValue)
            {
                if (maxUsageCount.Value < UsedCount)
                    return DiscountErrors.UsageLimitCannotBeLessThanUsedCount;

                var limitResult = SetUsageLimit(maxUsageCount.Value);
                if (limitResult.IsFailure) return limitResult.Error;
            }

            return Result.Success();
        }

        Code = code;
        Title = title;
        StartDate = startDate;
        EndDate = endDate;

        if (minimumOrderAmount != null)
        {
            var minResult = SetMinimumOrderAmount(minimumOrderAmount);
            if (minResult.IsFailure) return minResult.Error;
        }
        else
        {
            MinimumOrderAmount = null;
        }

        if (maximumDiscountAmount != null && Type == DiscountType.Percentage)
        {
            var maxResult = SetMaximumDiscountAmount(maximumDiscountAmount);
            if (maxResult.IsFailure) return maxResult.Error;
        }
        else
        {
            MaximumDiscountAmount = null;
        }

        if (maxUsageCount.HasValue)
        {
            var limitResult = SetUsageLimit(maxUsageCount.Value);
            if (limitResult.IsFailure) return limitResult.Error;
        }
        else
        {
            UsageLimit = null;
        }

        //RaiseDomainEvent(DiscountUpdatedDomainEvent.Create(Id, Code));

        return Result.Success();
    }

    /// <summary>
    /// Updates percentage value (only if not used yet)
    /// </summary>
    public Result UpdatePercentage(decimal percentage)
    {
        if (Type != DiscountType.Percentage)
            return DiscountErrors.NotPercentageDiscount;

        if (UsedCount > 0)
            return DiscountErrors.CannotChangeUsedDiscount;

        if (percentage <= 0 || percentage > 100)
            return DiscountErrors.InvalidPercentage;

        PercentageValue = percentage;
        return Result.Success();
    }

    /// <summary>
    /// Updates fixed amount value (only if not used yet)
    /// </summary>
    public Result UpdateFixedAmount(Money amount)
    {
        if (Type != DiscountType.FixedAmount)
            return DiscountErrors.NotFixedAmountDiscount;

        if (UsedCount > 0)
            return DiscountErrors.CannotChangeUsedDiscount;

        if (amount is null)
            return DiscountErrors.InvalidFixedAmount;

        if (amount.Value <= 0)
            return DiscountErrors.FixedAmountMustBePositive;

        FixedAmountValue = amount;
        return Result.Success();
    }
}
