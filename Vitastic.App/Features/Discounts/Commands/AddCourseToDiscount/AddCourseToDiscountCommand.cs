using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Discounts.Commands.AddCourseToDiscount
{
    public sealed record AddCourseToDiscountCommand(
        Guid DiscountId,
        Guid CourseId) : ICommand;
}