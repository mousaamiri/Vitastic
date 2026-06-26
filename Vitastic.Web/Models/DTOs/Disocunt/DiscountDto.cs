namespace Vitastic.Web.Models.DTOs.Disocunt;

// ======================== REQUESTS ========================

#region Request Models

/// <summary>
/// Request model for creating a fixed amount discount
/// </summary>
public class UpsertDiscountRequest
{
    public Guid? Id { get; set; }
    public string Title { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public decimal? Percentage { get; set; }
    public decimal? MaxDiscountAmount { get; set; }// max discount when is percentage
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? MaxUsageCount { get; set; }
    public int? MaxUsagePerUser { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
}
/// <summary>
/// Request model for extending discount end date
/// </summary>
public class ExtendDiscountRequest
{
    public DateTime EndDate { get; set; }
}

/// <summary>
/// Request model for setting maximum discount amount
/// </summary>
public class SetMaximumAmountRequest
{
    public decimal MaxAmount { get; set; }
}

/// <summary>
/// Request model for setting minimum order amount
/// </summary>
public class SetMinimumAmountRequest
{
    public decimal MinAmount { get; set; }
}

#endregion

// ======================== RESPONSES ========================

#region Response Models

/// <summary>
/// Detailed discount response model
/// </summary>
public class DiscountDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal? Value { get; set; }
    public decimal? Percentage { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? MaxUsageCount { get; set; }
    public int? MaxUsagePerUser { get; set; }
    public int CurrentUsageCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
public enum DiscountType
{
    None,
    Percentage = 1,
    FixedAmount = 2,
    FreeShipping = 3
}

/// <summary>
/// Summary discount response model
/// </summary>
public class DiscountDto
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

/// <summary>
/// Discount calculation result
/// </summary>
public class DiscountCalculationDto
{
    public Guid DiscountId { get; set; }
    public decimal OrderAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
}

#endregion
