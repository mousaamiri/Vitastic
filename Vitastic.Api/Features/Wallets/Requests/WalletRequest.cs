namespace Vitastic.Api.Features.Wallets.Requests;

public sealed record AddFundsRequest(
    decimal Amount,
    string? Description = null);

public sealed record CreateWalletRequest(
    Guid UserId) ;

public sealed record WithdrawFundsRequest(
    decimal Amount,
    string? Description = null);
