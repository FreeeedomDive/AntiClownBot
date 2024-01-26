namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;

public interface IMinecraftRegisterService
{
    public Task<bool> CreateOrChangeAccountAsync(Guid discordId, string username, string password);
}