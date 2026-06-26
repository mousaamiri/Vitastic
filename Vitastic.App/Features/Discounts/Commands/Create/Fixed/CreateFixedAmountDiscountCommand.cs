using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts.Enums;

namespace Vitastic.App.Features.Discounts.Commands.Create.Fixed;

public sealed record CreateFixedAmountDiscountCommand
    : ICommand<Guid>
{

    public string Code { get; set; }=string.Empty;
    public string Title { get; set; }=string.Empty;
    public DiscountScope Scope { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string Currency { get; set; }="IRT";
}
