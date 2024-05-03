namespace AntiClown.DiscordBot.Utility.Locks;

public interface ILocker
{
    Task AcquireAsync(string key);
    Task ReleaseAsync(string key);
}