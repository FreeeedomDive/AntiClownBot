using System;

namespace AntiClownBot
{
    public static class Randomizer
    {
        private static readonly Random Random = new Random();
        public static int GetRandomNumberBetween(int a, int b)
        {
            (a, b) = (Math.Min(a, b), Math.Max(a, b));
            return Random.Next(a, b);
        }

        public static bool FlipACoin()
        {
            return GetRandomNumberBetween(0, 2) == 1;
        }
    }
}