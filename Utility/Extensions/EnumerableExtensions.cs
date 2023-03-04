using AntiClown.Tools.Utility.Random;

namespace AntiClown.Tools.Utility.Extensions;

public static class EnumerableExtensions
{
    public static T SelectRandomItem<T>(this IEnumerable<T> enumerable)
    {
        var array = enumerable.ToArray();
        return array[Randomizer.GetRandomNumberBetween(0, array.Length)];
    }
}