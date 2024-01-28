using AntiClown.Entertainment.Api.Dto.MinecraftAuth;

namespace AntiClown.Entertainment.Api.Client.MinecraftAuth;

public interface IMinecraftRegisterClient
{
    Task<RegistrationStatusDto> Register(RegisterRequest request);
}