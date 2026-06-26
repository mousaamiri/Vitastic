using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Categories.Commands.UpdateDescription;

public sealed record UpdateCategoryDescriptionCommand(Guid Id ,string Description): ICommand;
