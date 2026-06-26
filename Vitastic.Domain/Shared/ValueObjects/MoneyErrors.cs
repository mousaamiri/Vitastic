using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Shared.ValueObjects
{
    public static class MoneyErrors
    {
        public static Error InvalidAmount(decimal amount) =>
           Error.Validation("Money.InvalidAmount", $"مقدار {amount} معتبر نیست");
        public static Error EmptyCurrency() =>
             Error.Validation("Money.EmptyCurrency", "واحد پولی نمی‌تواند خالی باشد.");
        public static Error InvalidCurrency(string currency) =>  Error.Validation("Money.InvalidCurrency",
            $"واحد پولی {currency} باید سه حرفی باشد.");
        public static Error InvalidCurrency() =>
             Error.Validation("Money.InvalidCurrency", "واحد پولی باید سه حرفی باشد.");
        public static Error NullMoney() =>
             Error.Validation("Money.Null", "مقدار پولی نمی‌تواند null باشد.");
        public static Error MismatchedCurrency() =>
             Error.Validation("Money.MismatchedCurrency", "واحدهای پولی باید یکسان باشند.");
        public static Result<Money> InsufficientFunds() =>
             Error.Validation("Money.InsufficientFunds", "موجودی کافی نیست.");
    }
}
