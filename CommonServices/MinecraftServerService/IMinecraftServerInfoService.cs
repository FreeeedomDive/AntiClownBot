using Dto.MinecraftServerDto;

namespace CommonServices.MinecraftServerService;

public interface IMinecraftServerInfoService
{
    public Task<MinecraftServerInfo?> ReadServerInfo(string serverAddress);
}