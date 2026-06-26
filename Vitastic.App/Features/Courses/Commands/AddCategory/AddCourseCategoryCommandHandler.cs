using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.AddCategory
{
    public sealed class AddCourseCategoryCommandHandler(
        ICourseRepository courseRepository,
        ICategoryRepository categoryRepository,
        ICourseCategoryRepository courseCategoryRepository)
        : ICommandHandler<AddCourseCategoryCommand>
    {
        public async Task<Result> Handle(AddCourseCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryId = CategoryId.CreateFrom(request.CategoryId);
            if (categoryId.IsFailure)
                return categoryId.Error;

            var categoryIsExist = await categoryRepository.IsExistAsync(categoryId.Value, cancellationToken);
            if (!categoryIsExist)
                return Error.NotFound("AddCourseCategoryCommand.CategoryNotFound", "دسته بندی مورد نظر یافت نشد");

            var courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;

            var courseIsExist = await courseRepository.IsExistAsync(courseIdResult.Value, cancellationToken);
            if (!courseIsExist)
                return Error.NotFound("AddCourseCategoryCommand.NotFound", "دوره مورد نظر یافت نشد");

            var connectionIsExist = courseCategoryRepository
                .RelationIsExist(courseIdResult.Value,categoryId.Value,cancellationToken).Result;
            if(connectionIsExist)
                return Error.Conflict("AddCourseCategoryCommand.CategoryAlreadyAssigned", "دوره قبلا به این دسته بندی متصل شده است.");

            var relation = CourseCategory.Create(courseIdResult.Value, categoryId.Value);
            await courseCategoryRepository.AddAsync(relation, cancellationToken);
            return Result.Success();
        }
    }
}
