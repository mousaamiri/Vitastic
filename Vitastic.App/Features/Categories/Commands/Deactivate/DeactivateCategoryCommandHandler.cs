using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Commands.Deactivate;

public sealed class DeactivateCategoryCommandHandler(ICategoryRepository repository)
    : ICommandHandler<DeactivateCategoryCommand>
{
    public async Task<Result> Handle(DeactivateCategoryCommand request, CancellationToken cancellationToken)
    {
        Result<CategoryId> categoryIdResult = CategoryId.CreateFrom(request.CategoryId);
        if (categoryIdResult.IsFailure)
            return categoryIdResult.Error;

        Category? category = await repository.FindAsync(categoryIdResult.Value,cancellationToken);
        if (category == null)
            return Error.NotFound("DeactivateCategoryCommand.NotFound","دسته مورد نظر یافت نشد. ");
        category.Deactivate();
        await repository.UpdateAsync(category, cancellationToken);
        return Result.Success();
    }
}
