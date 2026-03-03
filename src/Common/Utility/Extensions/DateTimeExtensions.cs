namespace AntiClown.Tools.Utility.Extensions;

public static class DateTimeExtensions
{
    public static bool IsNightTime(this DateTime dateTime)
    {
        // с поправкой на ЕКБ таймзону
        return dateTime.Hour is > 19 or < 4;
    }
}