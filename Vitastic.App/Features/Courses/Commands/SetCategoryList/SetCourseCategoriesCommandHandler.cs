using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.SetCategoryList
{
    public sealed class SetCourseCategoriesCommandHandler(ICourseRepository courseRepository
        ,ICourseCategoryRepository courseCategoryRepository)
        : ICommandHandler<SetCourseCategoriesCommand>
    {
        public async Task<Result> Handle(SetCourseCategoriesCommand request, CancellationToken cancellationToken)
        {
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            var courseIsExist = await courseRepository.IsExistAsync(courseIdResult.Value, cancellationToken);

            if (!courseIsExist)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            IEnumerable<CategoryId> categoryIds =
                request.CategoryIds.Select(id => CategoryId.CreateFrom(id).Value);

            await courseCategoryRepository.ReassignCourseCategoriesAsync(courseIdResult.Value,categoryIds, cancellationToken);
            return Result.Success();
        }
    }
}
