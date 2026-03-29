namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;

public class F1SeasonStats
{
    public DriverStatistics[] TenthPlacePointsRating { get; set; }
    public DriverStatistics[] MostPickedForTenthPlace { get; set; }
    public DriverStatistics[] TenthPickedButDnfed { get; set; }
    public DriverStatistics[] MostDnfDrivers { get; set; }
    public DriverStatistics[] MostPickedForDnf { get; set; }
    public LeadGapPredictionStats[] ClosestLeadGapPredictions { get; set; }
}