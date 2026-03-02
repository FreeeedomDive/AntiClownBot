namespace AntiClown.Api.Dto.Achievements;

public record UserAchievementDto
{
    public Guid Id { get; set; }
    public Guid AchievementId { get; set; }
    public Guid UserId { get; set; }
    public DateTime GrantedAt { get; set; }
}
