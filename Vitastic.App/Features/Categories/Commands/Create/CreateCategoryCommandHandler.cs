using AutoMapper;
using MediatR;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Categories.Commands.Create;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository repository,
    IMapper mapper)
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (command.ParentCategoryId is not null)
        {
            var parentIdResult = CategoryId.CreateFrom(command.ParentCategoryId.Value);
            if (parentIdResult.IsFailure)
                return parentIdResult.Error;
            var parent = await repository.FindAsync(parentIdResult.Value, cancellationToken);
            if(parent is null)
                return Error.NotFound("CreateCategoryCommand.ParentNotFound","دسته بندی والد یافت نشد.");
        }
        Result<Category> categoryResult = Category.Create(
            command.Name,
            command.Slug,
            command.DisplayOrder,
            command.ParentCategoryId);

        if (categoryResult.IsFailure)
            return categoryResult.Error;
        Category category = categoryResult.Value;
        category.SetDescription(command.Description);

        await repository.AddAsync(category, cancellationToken);

        return category.Id.Value;
    }
}
