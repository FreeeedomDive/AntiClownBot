/* Generated file */
namespace AntiClown.Entertainment.Api.Client.MinecraftAccount;

public interface IMinecraftAccountClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterResponse> RegisterAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.RegisterRequest registerRequest);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinResponse> SetSkinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ChangeSkinRequest changeSkinRequest);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.GetRegisteredUsersResponse> GetAllNicknamesAsync();
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasRegistrationResponse> HasRegistrationByDiscordUserAsync(System.Guid discordUserId);
}
