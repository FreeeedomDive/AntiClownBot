/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.MinecraftAccount;

public interface IMinecraftAccountClient
{
    Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterResponse> RegisterAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterRequest registerRequest);
    Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinResponse> SetSkinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinRequest changeSkinRequest);
    Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.GetRegisteredUsersResponse> GetAllNicknamesAsync();
    Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasRegistrationResponse> HasRegistrationByDiscordUserAsync(System.Guid discordUserId);
}
