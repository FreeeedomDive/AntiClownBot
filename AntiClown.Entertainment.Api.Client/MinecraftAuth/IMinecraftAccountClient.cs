using AntiClown.Entertainment.Api.Dto.MinecraftAuth;

namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public interface IMinecraftAccountClient
{
    Task<RegistrationStatusDto> Register(RegisterRequest request);
    Task<bool> SetSkinAsync(ChangeSkinRequest request);
    Task<string[]> GetAllNicknames();
    Task<bool> HasRegistrationByDiscordUser(Guid discordUserId);
}