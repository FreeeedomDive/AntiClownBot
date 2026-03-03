using AntiClown.DiscordBot.Releases.Domain;

namespace AntiClown.DiscordBot.Releases.Repositories;

public interface IReleasesRepository
{
    Task CreateAsync(ReleaseVersion releaseVersion);
    Task<ReleaseVersion?> ReadLastAsync();
}