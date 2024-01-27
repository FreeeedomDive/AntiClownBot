namespace AntiClown.Entertainment.Api.Dto.MinecraftAuth;

public class AuthResponseDto
{
    public string Username { get; set; } = null!;

    public string? UserUUID { get; set; } = null!;

    public string? AccessToken { get; set; } = null!;
}