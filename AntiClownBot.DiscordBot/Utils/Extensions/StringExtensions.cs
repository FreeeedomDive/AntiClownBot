namespace AntiClownDiscordBotVersion2.Utils.Extensions;

public static class StringExtensions
{
    public static string Multiply(this string str, int multiplier)
    {
        return string.Concat(Enumerable.Repeat(str, multiplier));
    }

    public static bool IsCommand(this string message, string commandPrefix)
    {
        return message.StartsWith(commandPrefix);
    }

    public static string GetCommandName(this string message, string commandPrefix)
    {
        return message.Split('\n')[0].Split(' ').First().ToLower()[commandPrefix.Length..];
    }
}