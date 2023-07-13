using AntiClown.Tools.Utility.Random;

namespace AntiClown.Tools.Utility.Extensions;

public static class EnumerableExtensions
{
    public static T SelectRandomItem<T>(this IEnumerable<T> enumerable)
    {
        var array = enumerable.ToArray();
        return array[Randomizer.GetRandomNumberBetween(0, array.Length)];
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var element in enumerable)
        {
            action(element);
        }
    }

    public static IEnumerable<T> Pipe<T>(this IEnumerable<T> enumerable, Action<T> func)
    {
        foreach (var element in enumerable)
        {
            func(element);
            yield return element;
        }
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.OrderBy(_ => Randomizer.GetRandomNumberBetween(0, 100_000_000));
    }

    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
    {
        var bucket = new T[size];
        var count = 0;

        foreach (var item in source)
        {
            bucket[count++] = item;
            if (count != size)
                continue;

            yield return bucket;

            bucket = new T[size];
            count = 0;
        }

        if (count > 0)
            yield return bucket.Take(count).ToArray();
    }
}