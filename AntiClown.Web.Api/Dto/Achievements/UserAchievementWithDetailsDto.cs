namespace AntiClown.Web.Api.Dto.Achievements;

public class UserAchievementWithDetailsDto
{
    public Guid Id { get; set; }
    public Guid AchievementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public DateTime GrantedAt { get; set; }
}
