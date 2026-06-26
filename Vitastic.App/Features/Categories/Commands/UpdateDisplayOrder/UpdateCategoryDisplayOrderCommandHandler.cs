using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Categories.Commands.UpdateName;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Commands.UpdateDisplayOrder;

public sealed class UpdateCategoryDisplayOrderCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategoryDisplayOrderCommand>
{
    public async Task<Result> Handle(UpdateCategoryDisplayOrderCommand command, CancellationToken cancellationToken)
    {
        Result<CategoryId> categoryIdResult = CategoryId.CreateFrom(command.Id);
        if (categoryIdResult.IsFailure) return categoryIdResult.Error;
        Category? category = await categoryRepository.FindAsync(categoryIdResult.Value, cancellationToken);
        if (category == null)
        {
            return Error.NotFound("UpdateCategoryDisplayOrderCommand.CategoryNotFound", "این دسته بندی یافت نشد. ");
        }

        category.UpdateDisplayOrder(command.DisplayOrder);

        await categoryRepository.UpdateAsync(category, cancellationToken);
        return Result.Success();
    }
}
