namespace AntiClownDiscordBotVersion2.Utils.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Shuffle<T>(
        this IEnumerable<T> items,
        IRandomizer randomizer
    )
    {
        return items.OrderBy(_ => randomizer.GetRandomNumberBetween(0, 1000000));
    }

    public static T SelectRandomItem<T>(
        this IEnumerable<T> items,
        IRandomizer randomizer
    )
    {
        var list = items.ToList();
        return list[randomizer.GetRandomNumberBetween(0, list.Count)];
    }

    public static Queue<T> WithoutItem<T>(this IEnumerable<T> queue, T removableItem)
    {
        var newQueue = new Queue<T>();
        foreach (var item in queue.Where(item => !item?.Equals(removableItem) ?? false))
        {
            newQueue.Enqueue(item);
        }

        return newQueue;
    }
        
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> function)
    {
        foreach (var item in items)
        {
            function(item);
            yield return item;
        }
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T, int> function)
    {
        var i = 0;
        foreach (var item in items)
        {
            function(item, i++);
            yield return item;
        }
    }
}