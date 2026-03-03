namespace AntiClown.DiscordBot.Extensions;

public static class DictionaryExtensions
{
    public static string ToStatsString<T1, T2>(this Dictionary<T1, T2> dictionary) where T1 : notnull
    {
        return string.Join("\n", dictionary.Select(kv => $"{kv.Key}: {kv.Value}"));
    }
}