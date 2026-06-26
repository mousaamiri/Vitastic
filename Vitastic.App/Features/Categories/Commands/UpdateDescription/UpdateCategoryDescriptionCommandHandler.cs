using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Commands.UpdateDescription;

public sealed class UpdateCategoryDescriptionCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategoryDescriptionCommand>
{
    public async Task<Result> Handle(UpdateCategoryDescriptionCommand command, CancellationToken cancellationToken)
    {
        Result<CategoryId> categoryIdResult = CategoryId.CreateFrom(command.Id);
        if (categoryIdResult.IsFailure) return categoryIdResult.Error;
        Category? category = await categoryRepository.FindAsync(categoryIdResult.Value, cancellationToken);
        if (category == null)
        {
            return Error.NotFound("UpdateCategoryDescriptionCommand.CategoryNotFound", "این دسته بندی یافت نشد. ");
        }

        category.SetDescription(command.Description);

        await categoryRepository.UpdateAsync(category, cancellationToken);
        return Result.Success();
    }
}
