using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Infra.Services.Base;

internal class PaymentGatewayService(HttpClient client, IConfiguration config) : IPaymentGatewayService
{
    private readonly string merchantId = config["Zarinpal:MerchantId"];
    private readonly string callbackUrl = config["Zarinpal:CallbackUrl"];

    public async Task<Result<PaymentGatewayResult>> CreatePaymentAsync(PaymentTransaction transaction,
        string requestCallBackUrl)
    {
        var request = new
        {
            merchant_id = merchantId,
            callback_url = string.IsNullOrEmpty(requestCallBackUrl) ? callbackUrl : requestCallBackUrl,
            amount = transaction.Amount.Value * 10,
            description = $"Payment for wallet {transaction.WalletId}"
        };
        HttpResponseMessage response =
            await client.PostAsJsonAsync("https://sandbox.zarinpal.com/pg/v4/payment/request.json", request);
        ZarinpalCreateResponse? json = await response.Content.ReadFromJsonAsync<ZarinpalCreateResponse>();
        if (json?.Data.Code != 100)
            return  Error.Failure("PaymentGatewayService.CreatePaymentAsync",
                json?.Errors.First() ?? "خطا در ایجاد درخواست پرداخت رخ داده است.");
        var authority = json.Data.Authority;
        var paymentUrl = $"https://sandbox.zarinpal.com/pg/StartPay/{authority}";
        return Result.Success(new PaymentGatewayResult(authority, paymentUrl));
    }

    public async Task<Result<int>> VerifyPaymentAsync(PaymentTransaction transaction, string status)
    {
        if (!status.Equals($"OK", StringComparison.OrdinalIgnoreCase))
            return  Error.Validation("PaymentGatewayService.VerifyPaymentAsync", "خطا در تایید پرداخت.");
        var request = new
        {
            merchant_id = config["Zarinpal:MerchantId"],
            amount = transaction.Amount.Value * 10,
            authority = transaction.PaymentInfo.Authority,
        };
        HttpResponseMessage response =
            await client.PostAsJsonAsync("https://sandbox.zarinpal.com/pg/v4/payment/verify.json", request);
        ZarinpalVerifyResponse? json = await response.Content.ReadFromJsonAsync<ZarinpalVerifyResponse>();
        return json?.Data?.Code == 100
            ? Result.Success(json.Data.RefId)
            :  Error.Failure("VerifyPaymentAsync.Zarinpal.VerifyFailed",
                json?.Errors?.First() ?? "خطا در تایید پرداخت رخ داده است.");
    }
}
