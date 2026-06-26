using AutoMapper;
using Vitastic.App.Features.Payments.Dtos;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.Enums;

namespace Vitastic.App.Features.Common.Mapping;

public class PaymentTransactionMappingProfile : Profile
{
    public PaymentTransactionMappingProfile()
    {
        // PaymentTransaction → PaymentTransactionDto
        CreateMap<PaymentTransaction, PaymentTransactionDto>();

        // PaymentTransaction → PaymentTransactionStatusDto
        CreateMap<PaymentTransaction, PaymentTransactionStatusDto>()
            .ForMember(dest => dest.IsPending, opt => opt.MapFrom(src => src.Status == TransactionStatus.Pending))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.Status == TransactionStatus.Completed))
            .ForMember(dest => dest.IsFailed, opt => opt.MapFrom(src => src.Status == TransactionStatus.Failed))
            .ForMember(dest => dest.IsReverted, opt => opt.MapFrom(src => src.Status == TransactionStatus.Reverted));
    }
}
