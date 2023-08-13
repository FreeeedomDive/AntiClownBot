using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.DiscordBot.Extensions;

public static class IntExtensions
{
    public static string ToPluralizedString(this int count, string singleForm, string severalForm, string manyForm)
    {
        var correctCount = Math.Abs(count % 100);
        if (correctCount is >= 10 and <= 20
            || (correctCount % 10 >= 5
                && correctCount % 10 <= 9)
            || correctCount % 10 == 0)
        {
            return $"{count} {manyForm}";
        }

        return correctCount % 10 == 1 ? $"{count} {singleForm}" : $"{count} {severalForm}";
    }

    public static string ToStringWithLeadingZeros(this int number, int totalNumbers)
    {
        var leadingZerosCount = totalNumbers - number.ToString().Length;
        return $"{"0".Multiply(leadingZerosCount)}{number}";
    }

    public static string AddSpaces(this int number, int totalLength, bool areLeadingSpaces = true)
    {
        return number.ToString().AddSpaces(totalLength, areLeadingSpaces);
    }
}