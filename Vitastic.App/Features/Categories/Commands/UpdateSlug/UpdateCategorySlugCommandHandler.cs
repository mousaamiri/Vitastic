using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Categories.Commands.UpdateName;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Commands.UpdateSlug;

public sealed class UpdateCategorySlugCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategorySlugCommand>
{
    public async Task<Result> Handle(UpdateCategorySlugCommand command, CancellationToken cancellationToken)
    {
        Result<CategoryId> categoryIdResult = CategoryId.CreateFrom(command.Id);
        if (categoryIdResult.IsFailure) return categoryIdResult.Error;
        Category? category = await categoryRepository.FindAsync(categoryIdResult.Value, cancellationToken);
        if (category == null)
        {
            return Error.NotFound("UpdateCategorySlugCommand.CategoryNotFound", "این دسته بندی یافت نشد. ");
        }

        category.UpdateSlug(command.Slug);

        await categoryRepository.UpdateAsync(category, cancellationToken);
        return Result.Success();
    }
}
