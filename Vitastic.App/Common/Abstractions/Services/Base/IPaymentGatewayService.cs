using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Abstractions.Services.Base;

public interface IPaymentGatewayService
{
    Task<Result<PaymentGatewayResult>> CreatePaymentAsync(PaymentTransaction transaction, string requestCallBackUrl);
    Task<Result<int>> VerifyPaymentAsync(PaymentTransaction transaction, string status);
}
