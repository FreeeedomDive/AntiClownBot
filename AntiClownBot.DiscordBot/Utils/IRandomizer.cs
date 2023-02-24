namespace AntiClownDiscordBotVersion2.Utils;

public interface IRandomizer
{
    int GetRandomNumberBetween(int a, int b);
    int GetRandomNumberBetweenIncludeRange(int a, int b);
    long GetRandomNumberBetweenIncludeRange(long a, long b);
    bool FlipACoin();
}