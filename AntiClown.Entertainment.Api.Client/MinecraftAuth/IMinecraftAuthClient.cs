/* Generated file */
namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public interface IMinecraftAuthClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthResponseDto> AuthAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthRequest authRequest);
    System.Threading.Tasks.Task<System.Boolean> JoinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.JoinRequest joinRequest);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinedResponseDto> HasJoinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinRequest hasJoinRequest);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileResponseDto> ProfileAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileRequest profileRequest);
    System.Threading.Tasks.Task<IEnumerable<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesResponseDto>> ProfilesAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesRequest profilesRequest);
}
