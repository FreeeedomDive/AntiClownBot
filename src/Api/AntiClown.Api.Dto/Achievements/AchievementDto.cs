namespace AntiClown.Api.Dto.Achievements;

public record AchievementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
}
