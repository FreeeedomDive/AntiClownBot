namespace AntiClown.DiscordBot.Extensions;

public static class StringExtensions
{
    public static string Repeat(this string input, int count)
    {
        return count == 0 ? "" : string.Concat(Enumerable.Repeat(input, count));
    }

    public static string AddSpaces(this string input, int totalLength, bool areLeadingSpaces = true)
    {
        var inputLength = input.Length;
        var spaces = " ".Repeat(totalLength - inputLength);
        return areLeadingSpaces ? $"{spaces}{input}" : $"{input}{spaces}";
    }
}