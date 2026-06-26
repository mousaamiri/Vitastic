using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Commands.AddCourseToDiscount
{
    public sealed class AddCourseToDiscountCommandHandler(
        IDiscountRepository discountRepository,
        ILogger<AddCourseToDiscountCommandHandler> logger)
        : ICommandHandler<AddCourseToDiscountCommand>
    {
        public async Task<Result> Handle(
            AddCourseToDiscountCommand command,
            CancellationToken ct)
        {
            var discountIdResult = DiscountId.CreateFrom(command.DiscountId);
            if (discountIdResult.IsFailure)
                return discountIdResult.Error;
            var discount = await discountRepository.FindAsync(
                discountIdResult.Value, ct);

            if (discount is null)
                return Error.NotFound("Discount", "تخفیف مورد نظر یافت نشد");

            var courseIdResult = CourseId.CreateFrom(command.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var result = discount.AddCourse(courseIdResult.Value);
            if (result.IsFailure)
                return result.Error;

            await discountRepository.UpdateAsync(discount, ct);

            logger.LogInformation(
                "Course added to discount - DiscountId: {DiscountId}, CourseId: {CourseId}",
                command.DiscountId, command.CourseId);

            return Result.Success();
        }
    }
}