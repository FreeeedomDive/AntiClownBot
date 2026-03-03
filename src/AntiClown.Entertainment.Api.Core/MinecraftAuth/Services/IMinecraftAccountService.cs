using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;

public interface IMinecraftAccountService
{
    Task<RegistrationStatus> CreateOrChangeAccountAsync(Guid discordId, string username, string password);
    Task<bool> SetSkinAsync(Guid discordId, string? skinUrl, string? capeUrl);
    Task<string[]> GetAllNicknames();
    Task<bool> HasRegistrationByDiscordUser(Guid discordUserId);
}