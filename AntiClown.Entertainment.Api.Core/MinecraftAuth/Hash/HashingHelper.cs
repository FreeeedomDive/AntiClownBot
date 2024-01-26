using System.Security.Cryptography;
using System.Text;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Hash;

public class HashingHelper
{
    public static string? Hash(string hashData)
    {
        if (hashData == null)
            throw new ArgumentNullException(nameof(hashData));
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashData));

        var builder = new StringBuilder();
        foreach (var b in bytes)
            builder.Append(b.ToString("x2"));

        return builder.ToString();
    }
}