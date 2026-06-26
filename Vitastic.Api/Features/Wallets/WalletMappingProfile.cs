using AutoMapper;
using Vitastic.Api.Features.Wallets.Requests;
using Vitastic.Api.Features.Wallets.Responses;
using Vitastic.App.Features.Wallets.Commands.AddFunds;
using Vitastic.App.Features.Wallets.Commands.Create;
using Vitastic.App.Features.Wallets.Commands.WithdrawFunds;
using Vitastic.App.Features.Wallets.Dtos;

namespace Vitastic.Api.Features.Wallets;

public class WalletMappingProfile:Profile
{
    public WalletMappingProfile()
    {
        CreateMap<WalletBalanceDto,WalletBalanceResponse>();
        CreateMap<WalletDto,WalletResponse>();
        CreateMap<WalletTransactionDto,WalletTransactionResponse>();
    }
}
