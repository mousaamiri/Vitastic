using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Wallet;

namespace Vitastic.Web.Models.ViewModels;

public sealed class WalletInfoModel
{
    public WalletDto WalletDto { get; set; }
        = new WalletDto(Guid.Empty, Guid.Empty, "??",0.0m,"", false, DateTime.UtcNow);
    public AddFundsRequest ChargeRequest { get; set; }=new AddFundsRequest();
    public PaginatedData<WalletTransactionDto> Transactions { get; set; }= new();
    public string CallbackUrl   { get; set; }= string.Empty;
}
