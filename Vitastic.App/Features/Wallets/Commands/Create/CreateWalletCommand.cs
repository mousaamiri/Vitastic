using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Wallets.Dtos;

namespace Vitastic.App.Features.Wallets.Commands.Create;

public sealed record CreateWalletCommand(
    Guid UserId,
    string? CurrencyCode = "IRT") : ICommand<Guid>;
