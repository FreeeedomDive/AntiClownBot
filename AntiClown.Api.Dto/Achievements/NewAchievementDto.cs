namespace AntiClown.Api.Dto.Achievements;

public record NewAchievementDto
{
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
}
