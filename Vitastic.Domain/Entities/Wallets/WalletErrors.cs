using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Wallets;

public static class WalletErrors
{
        public static Error InvalidUser => Error.Validation("Wallet.InvalidUser", "کاربر نامعتبر است.");
        public static Error InvalidCurrency => Error.Validation("Wallet.InvalidCurrency", "واحد پول نامعتبر است.");
        public static Error AmountMustBePositive => Error.Validation("Wallet.AmountMustBePositive", "مقدار باید مثبت باشد.");
        public static Error CurrencyMismatch(Currency currency,string currencyCode) => Error.Validation("Wallet.CurrencyMismatch", $"واحد پول تراکنش ({currencyCode}) با واحد پول کیف پول ({currency.Code}) مطابقت ندارد.");

        public static Error InsufficientBalance(decimal balanceAmount, decimal amountAmount)
        => Error.Validation("Wallet.InsufficientBalance", $"موجودی کیف پول ({balanceAmount}) برای انجام تراکنش ({amountAmount}) کافی نیست.");

        public static Error InvalidTransaction => Error.Validation("Wallet.InvalidTransaction", "تراکنش نامعتبر است.");
        public static Error TransactionNotBelongsToWallet => Error.Validation("Wallet.TransactionNotBelongsToWallet", "تراکنش به این کیف پول تعلق ندارد.");
        public static Error TransactionNotFound => Error.Validation("Wallet.TransactionNotFound", "تراکنش در لیست تراکنش های فعال کیف پول یافت نشد.");
        public static Error TransactionNotCompleted => Error.Validation("Wallet.Completed", "تراکنش در وضعیت کامل شده نیست.");
        public static Error OnlyCompletedCanRevert => Error.Validation("Wallet.OnlyCompletedCanRevert", "فقط تراکنش‌های تکمیل شده می‌توانند برگشت داده شوند.");

}
