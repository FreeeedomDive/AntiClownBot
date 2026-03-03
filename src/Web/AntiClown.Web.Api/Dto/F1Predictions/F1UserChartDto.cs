namespace AntiClown.Web.Api.Dto.F1Predictions;

public record F1UserChartDto
{
    public Guid UserId { get; set; }
    public int[] Points { get; set; }
}