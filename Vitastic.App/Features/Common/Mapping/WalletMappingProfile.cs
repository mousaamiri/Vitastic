using AutoMapper;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Wallets;

namespace Vitastic.App.Features.Common.Mapping;

public class WalletMappingProfile : Profile
{
    public WalletMappingProfile()
    {

        // Wallet → WalletDto
        CreateMap<Wallet, WalletDto>();

        // Wallet → WalletBalanceDto
        CreateMap<Wallet, WalletBalanceDto>()
            .ForMember(dest => dest.CanWithdraw, opt
                => opt.MapFrom(src => src.Balance.Value > 0));

        CreateMap<PaymentTransaction, WalletTransactionDto>()
            .ForMember(dest => dest.Authority,op=>
                op.MapFrom(src => src.PaymentInfo.Authority))
            .ForMember(dest => dest.RefId,op=>
                op.MapFrom(src => src.PaymentInfo.RefId))
            .ForMember(dest => dest.Gateway,op=>
                op.MapFrom(src => src.PaymentInfo.Gateway))
            .ForMember(dest => dest.PaidDate,op=>
                op.MapFrom(src => src.PaymentInfo.PaidDate))
            .ForMember(dest => dest.Description,op=>
                op.MapFrom(src => src.PaymentInfo.Description))
            ;
    }
}
