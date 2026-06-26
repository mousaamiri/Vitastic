using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Commands.UpdateList;

public record UpdateCategoriesListCommand(List<CategoryUpdateDto> Categories) : ICommand;

public sealed class UpdateCategoriesListCommandValidation : AbstractValidator<UpdateCategoriesListCommand>
{
}

public sealed class UpdateCategoriesListCommandHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<UpdateCategoriesListCommand>
{
    public async Task<Result> Handle(UpdateCategoriesListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Category> categories = await categoryRepository.GetAllAsync(cancellationToken);
            foreach (CategoryUpdateDto dto in request.Categories)
            {
                var category = categories.FirstOrDefault(c => c.Id == dto.Id);
                //Update name
                var updateNameResult = category.UpdateName(dto.Name);
                if (updateNameResult.IsFailure)
                    return updateNameResult.Error;
                //Update description
                if (!string.IsNullOrEmpty(dto.Description?.Trim()))
                {
                    var updateDescriptionResult = category.SetDescription(dto.Description);
                    if (updateDescriptionResult.IsFailure)
                        return updateDescriptionResult.Error;
                }

                //Update slug
                var updateSlugResult = category.UpdateSlug(dto.Slug);
                if (updateSlugResult.IsFailure)
                    return updateSlugResult.Error;
                //Update display order
                var updateDisplayOrderResult = category.UpdateDisplayOrder(dto.DisplayOrder);
                if (updateDisplayOrderResult.IsFailure)
                    return updateDisplayOrderResult.Error;
                //Update parent

                var updateParentResult = category
                    .SetParentCategory(dto.ParentCategoryId.HasValue
                        ? CategoryId.CreateFrom(dto.ParentCategoryId.Value).Value
                        : null);
                if (updateParentResult.IsFailure)
                    return updateParentResult.Error;
            }

            await categoryRepository.UpdateRangeAsync(categories, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
