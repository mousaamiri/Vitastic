using AutoMapper;
using Vitastic.Api.Features.Payments.Requests;
using Vitastic.Api.Features.Payments.Responses;
using Vitastic.App.Features.Payments.Commands.AssignInfo;
using Vitastic.App.Features.Payments.Commands.AssignPaymentToOrder;
using Vitastic.App.Features.Payments.Commands.Create;
using Vitastic.App.Features.Payments.Commands.Init;
using Vitastic.App.Features.Payments.Commands.Verify;
using Vitastic.App.Features.Payments.Dtos;
using InitializePaymentResult = Vitastic.App.Features.Payments.Dtos.InitializePaymentResult;

namespace Vitastic.Api.Features.Payments;

public class PaymentMappingProfile:Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<AssignPaymentInfoRequest, AssignPaymentInfoCommand>();
        CreateMap<CreatePaymentTransactionRequest, CreatePaymentTransactionCommand>();
        CreateMap<InitializePaymentRequest, InitializePaymentCommand>();
        CreateMap<PaymentVerificationResponse, PaymentVerificationResponse>();
        CreateMap<VerifyAndCompletePaymentRequest, VerifyAndCompletePaymentCommand>();

        CreateMap<PaymentTransactionResponse, PaymentTransactionDto>();
        CreateMap< PaymentVerificationResult, PaymentVerificationResponse>();
        CreateMap< InitializePaymentResult, InitializePaymentResponse>();
        CreateMap<PaymentTransactionStatusResponse, PaymentTransactionStatusDto>();
    }
}
