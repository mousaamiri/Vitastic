using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Categories.Commands.UpdateDisplayOrder;

public sealed record UpdateCategoryDisplayOrderCommand(Guid Id ,int DisplayOrder): ICommand;
