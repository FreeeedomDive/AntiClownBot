namespace AntiClownDiscordBotVersion2.Utils.Extensions;

public static class DateTimeExtensions
{
    public static bool IsNightTime(this DateTime dateTime)
    {
        return dateTime.Hour is >= 1 and < 9;
    }
}