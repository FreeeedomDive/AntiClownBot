namespace AntiClown.Web.Api.Dto.F1Predictions;

public record F1ChartsDto
{
    public F1UserChartDto[] UsersCharts { get; set; }
    public F1UserChartDto ChampionChart { get; set; }
}