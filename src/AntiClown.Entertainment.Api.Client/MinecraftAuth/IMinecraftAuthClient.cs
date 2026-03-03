/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public interface IMinecraftAuthClient
{
    Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthResponseDto> AuthAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.AuthRequest authRequest);
    Task<bool> JoinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.JoinRequest joinRequest);
    Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinedResponseDto> HasJoinAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.HasJoinRequest hasJoinRequest);
    Task<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileResponseDto> ProfileAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfileRequest profileRequest);
    Task<IEnumerable<AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesResponseDto>> ProfilesAsync(AntiClown.Entertainment.Api.Dto.MinecraftAuth.ProfilesRequest profilesRequest);
}
