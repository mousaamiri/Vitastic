using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Categories.Dtos;

namespace Vitastic.App.Features.Categories.Commands.Create;

public sealed record CreateCategoryCommand(
    string Name,
    string Slug,
    Guid? ParentCategoryId,
    int? DisplayOrder,
    string? Description
    ) : ICommand<Guid>;
