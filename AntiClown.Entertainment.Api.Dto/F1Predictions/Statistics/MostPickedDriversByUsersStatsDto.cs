namespace AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;

public class MostPickedDriversByUsersStatsDto
{
    public DriverStatisticsDto[] TenthPlacePickedDrivers { get; set; }
    public DriverStatisticsDto[] FirstDnfPickedDrivers { get; set; }
}