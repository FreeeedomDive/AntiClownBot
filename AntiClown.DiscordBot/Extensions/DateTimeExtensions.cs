namespace AntiClown.DiscordBot.Extensions;

public static class DateTimeExtensions
{
    public static TimeSpan GetDifferenceTimeSpan(this DateTime dateTime, DateTime other)
    {
        return dateTime > other ? dateTime - other : other - dateTime;
    }

    public static string ToTimeString(this DateTime dateTime)
    {
        return $"{dateTime.Hour}:{dateTime.Minute.ToStringWithLeadingZeros(2)}:{dateTime.Second.ToStringWithLeadingZeros(2)}";
    }

    public static DateTime ToYekaterinburgTime(this DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTime(
            dateTime, 
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Yekaterinburg")
        );
    }
}