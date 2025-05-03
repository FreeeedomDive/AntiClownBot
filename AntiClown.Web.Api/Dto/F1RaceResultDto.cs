using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Web.Api.Dto;

public class F1RaceResultDto
{
    public bool Success { get; set; }
    public F1PredictionRaceResultDto? Result { get; set; }
}