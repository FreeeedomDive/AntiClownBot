namespace AntiClown.DiscordBot.Dto.Members;

public class DiscordMemberDto
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? ServerName { get; set; }
    public string? AvatarUrl { get; set; }
}