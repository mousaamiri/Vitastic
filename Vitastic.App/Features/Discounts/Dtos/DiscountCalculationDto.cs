namespace Vitastic.App.Features.Discounts.Dtos
{
    public sealed record DiscountCalculationDto(
        Guid DiscountId,
        decimal OriginalAmount,
        decimal DiscountAmount,
        decimal FinalAmount,
        string Currency,
        bool IsValid,
        string? ErrorMessage);
}