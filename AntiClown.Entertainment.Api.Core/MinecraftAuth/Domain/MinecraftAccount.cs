namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;

public class MinecraftAccount
{
    public Guid Id { get; set; }

    public string Username { get; set; }

    public string? UsernameAndPasswordHash { get; set; }

    public string? AccessTokenHash { get; set; }

    public string? SkinUrl { get; set; }

    public string? CapeUrl { get; set; }

    public string DiscordId { get; set; }
}