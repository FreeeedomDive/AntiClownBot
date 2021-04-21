using System;

namespace AntiClownBot
{
    public static class Randomizer
    {
        public static int GetRandomNumberBetween(int a, int b)
        {
            return new Random().Next(a, b);
        }

        public static bool FlipACoin()
        {
            return GetRandomNumberBetween(0, 2) == 1;
        }
    }
}