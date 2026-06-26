namespace Vitastic.Web.Helper.Extensions;

public static class PerseanTranslateExtensions
{
    public static string CurrencyTranslate(this string currency)
    {
        return currency switch
        {
            "IRT" => "تومان",
            "IRR" => "ریال",
            _ => throw new ArgumentException("مقدار ارز نامعتبر است.")
        };
    }

    public static string UserStateTranslate(this bool isActive)
    {
        return isActive switch
        {
            true => "فعال",
            false => "غیره فعال"
        };
    }
}
