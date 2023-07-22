namespace AntiClown.DiscordBot.Extensions;

public static class StringExtensions
{
    public static string Repeat(this string input, int count)
    {
        return string.Join("", Enumerable.Repeat(0, count).Select(_ => input));
    }

    public static string AddSpaces(this string input, int totalLength, bool areLeadingSpaces = true)
    {
        var inputLength = input.Length;
        var spaces = " ".Repeat(totalLength - inputLength);
        return areLeadingSpaces ? $"{spaces}{input}" : $"{input}{spaces}";
    }
}