using System.Text;

namespace AntiClown.Tools.Utility.Extensions;

public static class DictionaryExtensions
{
    public static async Task<string> GetStats<T>(this Dictionary<T, int> dict, Func<T, Task<string>> func)
        where T : notnull
    {
        var list = dict.ToList();
        list.Sort((pair1, pair2) => -pair1.Value.CompareTo(pair2.Value));
        var sb = new StringBuilder();
        var i = 1;
        foreach (var (key, value) in list)
        {
            try
            {
                var modifiedKey = await func(key);
                sb.Append($"{i}: {modifiedKey} - {value}\n");
            }
            catch
            {
                sb.Append($"{i}: {key} - {value}\n");
            }

            if (i == 25)
            {
                break;
            }

            i++;
        }

        return sb.ToString();
    }

    public static void AddRecord<T>(this Dictionary<T, int> dict, T key, int value = 1) where T : notnull
    {
        if (dict.ContainsKey(key))
        {
            dict[key] += value;
        }
        else
        {
            dict.Add(key, value);
        }
    }

    public static void RemoveRecord<T>(this Dictionary<T, int> dict, T emoji) where T : notnull
    {
        if (!dict.ContainsKey(emoji))
        {
            return;
        }

        var value = dict[emoji];
        if (value == 1)
        {
            dict.Remove(emoji);
        }
        else
        {
            dict[emoji] = value - 1;
        }
    }

    public static void Add<T1, T2>(this Dictionary<T1, List<T2>> dictionary, T1 key, T2 value) where T1 : notnull
    {
        if (dictionary.TryGetValue(key, out var values))
        {
            values.Add(value);
        }
        else
        {
            dictionary[key] = new List<T2> { value };
        }
    }

    public static void Remove<T1, T2>(this Dictionary<T1, List<T2>> dictionary, T1 key, T2 value) where T1 : notnull
    {
        if (!dictionary.TryGetValue(key, out var values))
        {
            return;
        }

        values.Remove(value);
        if (values.Count == 0)
        {
            dictionary.Remove(key);
        }
    }
}