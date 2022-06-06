namespace AntiClownDiscordBotVersion2.Utils.Extensions;

public static class StringExtensions
{
    public static string Multiply(this string str, int multiplier)
    {
        return string.Concat(Enumerable.Repeat(str, multiplier));
    }
}