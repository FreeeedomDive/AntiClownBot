using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;

public interface IMinecraftRegisterService
{
    public Task<RegistrationStatus> CreateOrChangeAccountAsync(Guid discordId, string username, string password);
}