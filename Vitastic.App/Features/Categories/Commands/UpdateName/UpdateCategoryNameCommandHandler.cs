using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Commands.UpdateName;

public sealed class UpdateCategoryNameCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategoryNameCommand>
{
    public async Task<Result> Handle(UpdateCategoryNameCommand command, CancellationToken cancellationToken)
    {
        Result<CategoryId> categoryIdResult = CategoryId.CreateFrom(command.Id);
        if (categoryIdResult.IsFailure) return categoryIdResult.Error;
        Category? category = await categoryRepository.FindAsync(categoryIdResult.Value, cancellationToken);
        if (category == null)
        {
            return Error.NotFound("UpdateCategoryNameCommand.CategoryNotFound", "این دسته بندی یافت نشد. ");
        }

        category.UpdateName(command.Name);

        await categoryRepository.UpdateAsync(category, cancellationToken);
        return Result.Success();
    }
}
