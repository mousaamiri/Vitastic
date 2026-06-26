namespace Vitastic.Api.Features.Discounts.Responses;

public sealed record DiscountResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string? Type { get; init; }
    public string Scope { get; init; } = null!;
    public decimal Value { get; init; }
    public string Currency { get; init; } = null!;
    public bool IsActive { get; init; }
    public bool IsExpired { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
    public int UsedCount { get; init; }
    public int? UsageLimit { get; init; }
}

public sealed record DiscountDetailResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public string Type { get; init; } = null!;
    public string Scope { get; init; } = null!;
    public decimal Value { get; init; }
    public string Currency { get; init; } = null!;
    public decimal? MinimumOrderAmount { get; init; }
    public decimal? MaximumDiscountAmount { get; init; }
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
    public bool IsActive { get; init; }
    public bool IsSingleUse { get; init; }
    public int UsedCount { get; init; }
    public int? UsageLimit { get; init; }
    public int RemainingUsage { get; init; }
    public List<Guid> ApplicableCourseIds { get; init; } = [];
    public List<Guid> ApplicableCategoryIds { get; init; } = [];
    public List<Guid> ApplicableInstructorIds { get; init; } = [];
}

public sealed record DiscountCalculationResponse(
    Guid DiscountId,
    decimal OriginalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    string Currency,
    bool IsValid,
    string? ErrorMessage);
