namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;

public class AuthResponse
{
    public string Username { get; set; } = null!;
    public string? UserId { get; set; } = null!;
    public string? AccessToken { get; set; } = null!;
}
