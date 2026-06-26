using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Orders.Commands.AddItemToOrder
{
    public sealed class AddItemToOrderCommandHandler
    (IOrderRepository orderRepository,
        ICourseRepository courseRepository)
        : ICommandHandler<AddItemToOrderCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddItemToOrderCommand command, CancellationToken cancellationToken)
        {
            //Order id
            Result<OrderId> orderIdResult = OrderId.CreateFrom(command.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            // Get Order
            Order? order = await orderRepository.FindAsync(
                orderIdResult.Value,
                cancellationToken);
            if (order is null)
                return Error.NotFound();
            //Course id
            Result<CourseId> courseIdResult = CourseId.CreateFrom(command.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            //Get course
            Course? course = await courseRepository.FindAsync(courseIdResult.Value, cancellationToken);
            if (course is null)
                return Error.NotFound("AddItemToOrderCommand.CourseNotFound", "دوره ای با این شناسه یافت نشد. ");
            //Check item is in the order for this user
            var coursePurchased = await orderRepository.ItemIsPurchasedAsync(course.Id,order.UserId, cancellationToken);
            if (coursePurchased)
                return Error.Conflict("AddItemToOrderCommand.UserAlreadyPurchasedThisCourse", "شما پیش از این ، این دوره را خریداری کرده اید.یا در سفارش شما قرار دارد.");
            //Get Instructor name
            var instructorName = await courseRepository.GetInstructorName(course.Id, cancellationToken);
            Result<FullName> instructorNameResult = FullName.Create(instructorName);
            if (instructorNameResult.IsFailure)
                return instructorNameResult.Error;
            // Add Item to Order
            Result<OrderItem> itemResult = order.AddItem(course.Id
                ,course.Title
                ,course.ImageName
                ,instructorNameResult.Value
                ,course.Price);

            if (itemResult.IsFailure)
               return itemResult.Error;

            OrderItem item = itemResult.Value;

            await orderRepository.UpdateAsync(order, cancellationToken);


            return item.Id.Value;
        }
    }
}
