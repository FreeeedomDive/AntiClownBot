namespace AntiClown.DiscordBot.Releases.Services;

public interface IReleasesService
{
    Task NotifyIfNewVersionAvailableAsync();
}