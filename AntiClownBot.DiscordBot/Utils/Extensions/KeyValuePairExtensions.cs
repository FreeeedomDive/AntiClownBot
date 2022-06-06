namespace AntiClownDiscordBotVersion2.Utils.Extensions;

public static class KeyValuePairExtensions
{
    public static string ToString<T1, T2>(this KeyValuePair<T1, T2> pair)
    {
        var (key, value) = pair;
        return $"{key} : {value}";
    }
}