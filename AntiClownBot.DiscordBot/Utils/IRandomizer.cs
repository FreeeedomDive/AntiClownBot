namespace AntiClownDiscordBotVersion2.Utils;

public interface IRandomizer
{
    int GetRandomNumberBetween(int a, int b);
    int GetRandomNumberBetweenIncludeRange(int a, int b);
    bool FlipACoin();
}