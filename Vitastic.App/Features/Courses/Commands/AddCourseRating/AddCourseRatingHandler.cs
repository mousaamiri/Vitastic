using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.AddCourseRating
{
    public sealed class AddCourseRatingHandler(
        ICourseRepository courseRepository,
        ICourseRatingRepository ratingRepository)
        : ICommandHandler<AddCourseRatingCommand>
    {
        public async Task<Result> Handle(AddCourseRatingCommand request, CancellationToken ct)
        {
            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;

            var courseIsExisted = await courseRepository.IsExistAsync(courseIdResult.Value, ct);
            if (!courseIsExisted)
                return Error.NotFound("AddCourseRatingHandler.CourseNotFound", "دوره مورد نظر یافت نشد.");

            var userIdResult = UserId.CreateFrom(request.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;

            var existingRating = await ratingRepository.FindAsync(courseIdResult.Value, userIdResult.Value, ct);
            if (existingRating is not null)
                return Error.Conflict("AddCourseRatingHandler.RelationIsExist", "شما قبلاً به این دوره امتیاز داده‌اید.");

            var ratingResult = CourseRating.Create(
                courseIdResult.Value,
                userIdResult.Value,
                request.RatingValue,
                request.Comment);

            if (ratingResult.IsFailure)
                return ratingResult.Error;

            await ratingRepository.AddAsync(ratingResult.Value, ct);

            return Result.Success();
        }
    }
}