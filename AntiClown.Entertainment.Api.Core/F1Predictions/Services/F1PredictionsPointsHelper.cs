﻿namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public static class F1PredictionsPointsHelper
{
    public const int PointsForCorrectFirstDnfPrediction = 5;

    public static readonly Dictionary<int, int> PointsDistribution = new()
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 4 },
        { 4, 6 },
        { 5, 8 },
        { 6, 10 },
        { 7, 12 },
        { 8, 15 },
        { 9, 18 },
        { 10, 25 },
        { 11, 18 },
        { 12, 15 },
        { 13, 12 },
        { 14, 10 },
        { 15, 8 },
        { 16, 6 },
        { 17, 4 },
        { 18, 2 },
        { 19, 1 },
        { 20, 1 },
    };
}