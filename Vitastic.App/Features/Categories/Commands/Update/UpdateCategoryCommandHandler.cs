using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        Result<CategoryId> categoryIdResult = CategoryId.CreateFrom(command.Id);
        if (categoryIdResult.IsFailure) return categoryIdResult.Error;
        Category? category = await categoryRepository.FindAsync(categoryIdResult.Value, cancellationToken);
        if (category == null)
        {
            return Error.NotFound("UpdateCategoryCommand.CategoryNotFound", "این دسته بندی یافت نشد. ");
        }

        var updateNameResult = category.UpdateName(command.Name);
        if (updateNameResult.IsFailure)
            return updateNameResult.Error;
        var updateSlugResult = category.UpdateSlug(command.Slug);
        if (updateSlugResult.IsFailure)
            return updateSlugResult.Error;
        if (command.DisplayOrder.HasValue)
        {
            var updateDisplayOrder = category.UpdateDisplayOrder(command.DisplayOrder.Value);
            if (updateDisplayOrder.IsFailure)
                return updateDisplayOrder.Error;
        }
        //set parent
        var setParentResult = category.SetParentCategory(command.ParentCategoryId.HasValue?
            CategoryId.CreateFrom(command.ParentCategoryId.Value).Value : null);
        if (setParentResult.IsFailure)
            return setParentResult.Error;

        await categoryRepository.UpdateAsync(category, cancellationToken);
        return Result.Success();
    }
}
