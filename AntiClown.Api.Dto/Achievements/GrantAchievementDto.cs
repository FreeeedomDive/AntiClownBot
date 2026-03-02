namespace AntiClown.Api.Dto.Achievements;

public record GrantAchievementDto
{
    public Guid UserId { get; set; }
    public DateTime GrantedAt { get; set; }
}
