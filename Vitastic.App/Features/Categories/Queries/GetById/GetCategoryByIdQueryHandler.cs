using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Queries.GetById;

public sealed class GetCategoryByIdQueryHandler(ICategoryQueryService categoryRepository)
    : IQueryHandler<GetCategoryByIdQuery, CategoryDetailDto>
{
    public async Task<Result<CategoryDetailDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var categoryIdResult = CategoryId.CreateFrom(request.CategoryId);
        if (categoryIdResult.IsFailure)
            return categoryIdResult.Error;
        CategoryDetailDto? category =
            await categoryRepository.GetDetailAsync(categoryIdResult.Value, cancellationToken);
        if (category is null)
            return Error.NotFound("GetCategoryQueryHandler.GetCategoryById", "دسته یافت نشد.");
        return Result.Success(category);
    }
}
