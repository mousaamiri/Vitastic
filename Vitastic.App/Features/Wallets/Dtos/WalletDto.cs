namespace Vitastic.App.Features.Wallets.Dtos;

public sealed record WalletDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
