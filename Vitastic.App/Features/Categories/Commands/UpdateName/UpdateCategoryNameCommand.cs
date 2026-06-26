using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Categories.Commands.UpdateName;

public sealed record UpdateCategoryNameCommand(Guid Id ,string Name): ICommand;
