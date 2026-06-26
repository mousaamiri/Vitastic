using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Categories.Commands.Update;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Slug,
    int? DisplayOrder,
    Guid? ParentCategoryId,
    string? Description) : ICommand;
