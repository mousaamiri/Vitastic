namespace Vitastic.App.Features.Wallets.Dtos
{
    public sealed class WalletBalanceDto
    {
        public Guid Id{get; private set; }
        public decimal Balance{get; private set; }
        public string Currency { get; private set; } = null!;
        public bool CanWithdraw { get; private set; }
    }
}
