namespace AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;

public class F1SeasonStatsDto
{
    public DriverStatisticsDto[] TenthPlacePointsRating { get; set; }
    public DriverStatisticsDto[] MostPickedForTenthPlace { get; set; }
    public DriverStatisticsDto[] TenthPickedButDnfed { get; set; }
    public DriverStatisticsDto[] MostDnfDrivers { get; set; }
    public DriverStatisticsDto[] MostPickedForDnf { get; set; }
    public LeadGapPredictionStatsDto[] ClosestLeadGapPredictions { get; set; }
}
