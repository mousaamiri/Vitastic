using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.RemoveCategory
{
    public sealed class RemoveCourseCategoryCommandHandler
        (ICourseRepository courseRepository,
            ICategoryRepository categoryRepository,
            ICourseCategoryRepository courseCategoryRepository)
        : ICommandHandler<RemoveCourseCategoryCommand>
    {

        public async Task<Result> Handle(RemoveCourseCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryId = CategoryId.CreateFrom(request.CategoryId);
            if (categoryId.IsFailure)
                return categoryId.Error;

            var categoryIsExist = await categoryRepository.IsExistAsync(categoryId.Value, cancellationToken);
            if (!categoryIsExist)
                return Error.NotFound("RemoveCourseCategoryCommand.CategoryNotFound", "دسته بندی مورد نظر یافت نشد");

            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;

            var courseIsExist = await courseRepository.IsExistAsync(courseIdResult.Value, cancellationToken);
            if (!courseIsExist)
                return Error.NotFound("RemoveCourseCategoryCommand.NotFound", "دوره مورد نظر یافت نشد");

            var connectionIsExist = courseCategoryRepository
                .RelationIsExist(courseIdResult.Value,categoryId.Value,cancellationToken).Result;
            if(!connectionIsExist)
                return Error.NotFound("RemoveCourseCategoryCommand.CourseCategoryRelationNoFound",
                    "اتصال بین دوره و دسته بندی یافت نشد.");

            var relation = await courseCategoryRepository
                .FindAsync(courseIdResult.Value, categoryId.Value,cancellationToken);
            await courseCategoryRepository.DeleteAsync(relation, cancellationToken);
            return Result.Success();
        }
    }
}
