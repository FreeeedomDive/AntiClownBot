namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;

public class F1SeasonStats
{
    public DriverStatistics[] TenthPlacePointsRating { get; set; }
    public DriverStatistics[] MostPickedForTenthPlace { get; set; }
    public DriverStatistics[] TenthPickedButDnfed { get; set; }
    public DriverStatistics[] DriverOwnTenthPlacePoints { get; set; }
    public DriverStatistics[] TenthPlacePredictionEfficiency { get; set; }
    public DriverStatistics[] MostDnfDrivers { get; set; }
    public DriverStatistics[] MostPickedForDnf { get; set; }
    public LeadGapPredictionStats[] ClosestLeadGapPredictions { get; set; }
    public DriverStatistics[] SafetyCarPickCounts { get; set; }
    public DriverStatistics[] SafetyCarActualCounts { get; set; }
    public DriverStatistics[] SafetyCarCorrectGuesses { get; set; }
}
