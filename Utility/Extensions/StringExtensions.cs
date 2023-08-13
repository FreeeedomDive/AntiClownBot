namespace AntiClown.Tools.Utility.Extensions;

public static class StringExtensions
{
    public static string Multiply(this string str, int multiplier)
    {
        return string.Concat(Enumerable.Repeat(str, multiplier));
    }
}