namespace AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;

public class LeadGapPredictionStatsDto
{
    public Guid UserId { get; set; }
    public string RaceName { get; set; }
    public decimal PredictedGap { get; set; }
    public decimal ActualGap { get; set; }
    public decimal Difference { get; set; }
}
