using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Categories.Commands.Deactivate;

public record DeactivateCategoryCommand(Guid CategoryId) : ICommand;
