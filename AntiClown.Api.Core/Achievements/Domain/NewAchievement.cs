namespace AntiClown.Api.Core.Achievements.Domain;

public record NewAchievement
{
    public string Name { get; set; } = string.Empty;
    public byte[] Logo { get; set; } = [];
}
