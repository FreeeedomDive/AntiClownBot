namespace AntiClownDiscordBotVersion2.Utils;

public class Randomizer : IRandomizer
{
    public Randomizer(Random random)
    {
        this.random = random;
    }
    
    public int GetRandomNumberBetween(int a, int b)
    {
        (a, b) = (Math.Min(a, b), Math.Max(a, b));
        return random.Next(a, b);
    }

    public int GetRandomNumberBetweenIncludeRange(int a, int b)
    {
        (a, b) = (Math.Min(a, b), Math.Max(a, b));
        return random.Next(a, b + 1);
    }

    public long GetRandomNumberBetweenIncludeRange(long a, long b)
    {
        (a, b) = (Math.Min(a, b), Math.Max(a, b));
        return random.NextInt64(a, b + 1);
    }

    public bool FlipACoin()
    {
        return GetRandomNumberBetween(0, 2) == 1;
    }

    private readonly Random random;
}