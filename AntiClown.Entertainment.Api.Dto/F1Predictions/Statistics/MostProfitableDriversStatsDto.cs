namespace AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;

public class MostProfitableDriversStatsDto
{
    public DriverStatisticsDto[] TenthPlacePoints { get; set; }
    public DriverStatisticsDto[] TenthPlaceCount { get; set; }
    public DriverStatisticsDto[] FirstDnfCount { get; set; }
}