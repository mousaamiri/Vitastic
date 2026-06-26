namespace Vitastic.Web.Helper;

public static class PersianCalendar
{
    public static string DateToPersian(this DateTimeOffset date)
    {
        var persianCalendar = new System.Globalization.PersianCalendar();
        var year = persianCalendar.GetYear(date.DateTime);
        var month = persianCalendar.GetMonth(date.DateTime).ToString("00");
        var day = persianCalendar.GetDayOfMonth(date.DateTime).ToString("00");

        return $"{year}/{month}/{day}";
    }
}
