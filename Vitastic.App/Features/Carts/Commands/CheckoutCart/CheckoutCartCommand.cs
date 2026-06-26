using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Carts.Commands.CheckoutCart;

public sealed record CheckoutCartCommand(Guid UserId) : ICommand<Guid>;

