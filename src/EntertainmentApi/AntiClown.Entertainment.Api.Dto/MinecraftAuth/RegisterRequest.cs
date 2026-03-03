namespace AntiClown.Entertainment.Api.Dto.MinecraftAuth;

public class RegisterRequest
{
    public Guid DiscordId { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}