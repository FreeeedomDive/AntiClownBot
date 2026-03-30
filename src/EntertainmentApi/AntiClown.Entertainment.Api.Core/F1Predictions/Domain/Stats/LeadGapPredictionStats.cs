namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;

public class LeadGapPredictionStats
{
    public Guid UserId { get; set; }
    public string RaceName { get; set; }
    public decimal PredictedGap { get; set; }
    public decimal ActualGap { get; set; }
    public decimal Difference { get; set; }
}
