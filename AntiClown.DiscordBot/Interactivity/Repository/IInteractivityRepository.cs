using AntiClown.DiscordBot.Interactivity.Domain;

namespace AntiClown.DiscordBot.Interactivity.Repository;

public interface IInteractivityRepository
{
    Task CreateAsync<T>(Interactivity<T> interactivity);
    Task UpdateAsync<T>(Interactivity<T> interactivity);
    Task<Interactivity<T>?> TryReadAsync<T>(Guid id);
}