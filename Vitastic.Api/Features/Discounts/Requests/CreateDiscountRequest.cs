namespace Vitastic.Api.Features.Discounts.Requests;
public class UpsertDiscountRequest
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
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
public sealed record ExtendDiscountRequest(DateTimeOffset EndDate);
public sealed record SetMaximumAmountRequest(decimal MaxAmount);
public sealed record SetMinimumAmountRequest(decimal MinAmount);

public enum DiscountScope
{
    Global = 1,
    SpecificCourses = 2,
    SpecificCategories = 3,
    SpecificInstructors = 4
}
