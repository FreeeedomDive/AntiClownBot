﻿namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;

public class F1DnfPrediction
{
    public F1Driver[]? DnfPickedDrivers { get; set; }
    public bool NoDnfPredicted { get; set; }
}