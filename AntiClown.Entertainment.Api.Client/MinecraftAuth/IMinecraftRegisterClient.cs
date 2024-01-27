using AntiClown.Entertainment.Api.Dto.MinecraftAuth;

namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public interface IMinecraftRegisterClient
{
    Task<bool> Register(RegisterRequest request);
}