﻿namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1RaceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsOpened { get; set; }
    public List<F1PredictionDto> Predictions { get; set; }
}