using System.Security.Cryptography;
using System.Text;

namespace AntiClown.Tools.Utility.Extensions;

public static class StringExtensions
{
    public static string Multiply(this string str, int multiplier)
    {
        return string.Concat(Enumerable.Repeat(str, multiplier));
    }

    public static Guid GetDeterministicGuid(this string input)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }
}