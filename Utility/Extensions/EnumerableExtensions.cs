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
}