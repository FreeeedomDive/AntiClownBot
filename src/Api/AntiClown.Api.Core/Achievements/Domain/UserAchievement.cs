namespace AntiClown.Api.Core.Achievements.Domain;

public record UserAchievement
{
    public Guid Id { get; set; }
    public Guid AchievementId { get; set; }
    public Guid UserId { get; set; }
    public DateTime GrantedAt { get; set; }
}
