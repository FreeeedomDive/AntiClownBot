namespace AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;

public class F1SeasonStatsDto
{
    public DriverStatisticsDto[] TenthPlacePointsRating { get; set; }
    public DriverStatisticsDto[] MostPickedForTenthPlace { get; set; }
    public DriverStatisticsDto[] TenthPickedButDnfed { get; set; }
    public DriverStatisticsDto[] DriverOwnTenthPlacePoints { get; set; }
    public DriverStatisticsDto[] TenthPlacePredictionEfficiency { get; set; }
    public DriverStatisticsDto[] MostDnfDrivers { get; set; }
    public DriverStatisticsDto[] MostPickedForDnf { get; set; }
    public UserStatisticsDto[] TenthPlaceDnfAntiRating { get; set; }
    public LeadGapPredictionStatsDto[] ClosestLeadGapPredictions { get; set; }
    public DriverStatisticsDto[] SafetyCarPickCounts { get; set; }
    public DriverStatisticsDto[] SafetyCarActualCounts { get; set; }
    public DriverStatisticsDto[] SafetyCarCorrectGuesses { get; set; }
}
