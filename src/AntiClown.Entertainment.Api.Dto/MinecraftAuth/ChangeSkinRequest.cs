namespace AntiClown.Entertainment.Api.Dto.MinecraftAuth;

public class ChangeSkinRequest
{
    public Guid DiscordUserId { get; set; }
    public string? SkinUrl { get; set; }
    public string? CapeUrl { get; set; }
}