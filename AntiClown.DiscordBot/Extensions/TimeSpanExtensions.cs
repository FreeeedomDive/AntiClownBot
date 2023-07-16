using System.Text;

namespace AntiClown.DiscordBot.Extensions;

public static class TimeSpanExtensions
{
    public static string ToTimeDiffString(this TimeSpan timeSpan)
    {
        if (timeSpan.Days != 0)
        {
            return $"более чем {timeSpan.Days.ToPluralizedString("день", "дня", "дней")}";
        }

        var sb = new StringBuilder();
        if (timeSpan.Hours != 0)
        {
            sb.Append(timeSpan.Hours.ToPluralizedString("час", "часа", "часов")).Append(' ');
        }

        if (timeSpan.Minutes != 0)
        {
            sb.Append(timeSpan.Minutes.ToPluralizedString("минуту", "минуты", "минут")).Append(' ');
        }

        if (timeSpan.Seconds != 0)
        {
            sb.Append(timeSpan.Seconds.ToPluralizedString("секунду", "секунды", "секунд")).Append(' ');
        }

        if (timeSpan is { Hours: 0, Minutes: 0, Seconds: 0 })
        {
            sb.Append(timeSpan.Milliseconds.ToPluralizedString("миллисекунду", "миллисекунды", "миллисекунд"));
        }

        return sb.ToString();
    }
}