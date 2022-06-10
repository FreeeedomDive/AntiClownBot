using System;
using System.Linq;

namespace AntiClownBot.Helpers
{
    public static class StringExtension
    {
        public static string Multiply(this string str, int multiplier)
        {
            return string.Concat(Enumerable.Repeat(str, multiplier));
        }
    }
}