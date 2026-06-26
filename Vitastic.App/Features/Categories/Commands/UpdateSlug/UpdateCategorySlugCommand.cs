using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Categories.Commands.UpdateSlug;

public sealed record UpdateCategorySlugCommand(Guid Id ,string Slug): ICommand;
