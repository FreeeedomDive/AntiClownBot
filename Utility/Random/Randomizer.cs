﻿namespace AntiClown.Tools.Utility.Random;

public static class Randomizer
{
    private static readonly System.Random Random = new System.Random();
    public static int GetRandomNumberBetween(int a, int b)
    {
        (a, b) = (Math.Min(a, b), Math.Max(a, b));
        return Random.Next(a, b);
    }

    public static int GetRandomNumberBetweenIncludeRange(int a, int b) => GetRandomNumberBetween(a, b + 1);

    public static bool CoinFlip()
    {
        return GetRandomNumberBetween(0, 2) == 1;
    }
}