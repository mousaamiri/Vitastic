using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Discounts.Enums;

namespace Vitastic.App.Features.Discounts.Commands.Create.Percentage;

public sealed record CreatePercentageDiscountCommand : ICommand<Guid>
{
    public string Code { get; set; }=string.Empty;
    public string Title { get; set; }=string.Empty;
    public DiscountScope Scope { get; set; }
    public decimal Percentage { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
}

